namespace Contented.Core.Synology
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Security;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class SynologyApi
    {
        private readonly HttpClient httpClient;
        private string sessionId;

        public SynologyApi(
            HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IObservable<Unit> Authenticate(string user, SecureString password)
        {
            this.sessionId = Guid.NewGuid().ToString();
            return httpClient
                .GetAsync($"{httpClient.BaseAddress}/auth.cgi?api=SYNO.API.Auth&version=4&method=login&account={EscapeParameter(user)}&passwd={EscapeParameter(password.ToUnsecureString())}&session={this.sessionId}&format=cookie")
                .ToObservable()
                .ValidateAndParseResponse()
                .ToSignal();
        }

        public IObservable<Unit> LogOut() =>
            httpClient
                .GetAsync($"{httpClient.BaseAddress}/auth.cgi?api=SYNO.API.Auth&version=1&method=logout&session={this.sessionId}")
                .ToObservable()
                .ValidateAndParseResponse()
                .ToSignal();

        public IObservable<Unit> CreateDownloadTask(string uri, string destination) =>
            httpClient
                .GetAsync($"{httpClient.BaseAddress}/DownloadStation/task.cgi?api=SYNO.DownloadStation.Task&version=1&method=create&uri={uri}&destination={EscapeParameter(destination)}")
                .ToObservable()
                .ValidateAndParseResponse()
                .ToSignal();

        public IObservable<IImmutableList<DownloadDto>> GetDownloadTasks() =>
            httpClient
                .GetAsync($"{httpClient.BaseAddress}/DownloadStation/task.cgi?api=SYNO.DownloadStation.Task&version=1&method=list&additional=detail,transfer")
                .ToObservable()
                .ValidateAndParseResponse()
                .Select(jobject => jobject["data"]["tasks"])
                .Select(jobject => JsonConvert.DeserializeObject<IImmutableList<DownloadDto>>(jobject.ToString()));

        public IObservable<IImmutableList<string>> RemoveDownloadTasks(IImmutableList<string> ids, bool forceComplete) =>
            httpClient
                .GetAsync($"{httpClient.BaseAddress}/DownloadStation/task.cgi?api=SYNO.DownloadStation.Task&version=1&method=delete&id={ids.Join(",")}&force_complete={forceComplete}")
                .ToObservable()
                .ValidateAndParseResponse()
                .Select(jobject => jobject["data"].Children().Select(child => child["id"].ToString()).ToImmutableList());

        public IObservable<IImmutableList<string>> CreateFolder(string rootPath, string name, bool forceCreateParent) =>
            httpClient
                .GetAsync($"{httpClient.BaseAddress}/entry.cgi?api=SYNO.FileStation.CreateFolder&version=2&method=create&folder_path=\"{rootPath}\"&name=\"{name}\"")
                .ToObservable()
                .ValidateAndParseResponse()
                .Select(jobject => jobject["data"]["folders"].Children().Select(child => child["path"].ToString()).ToImmutableList());

        public IObservable<Unit> Configure(int? maxDownloadSpeed, int? maxUploadSpeed) =>
            httpClient
                .GetAsync($"{httpClient.BaseAddress}/DownloadStation/info.cgi?api=SYNO.DownloadStation.Info&version=1&method=setserverconfig{GetOptionalParameter("bt_max_download", maxDownloadSpeed?.ToString())}{GetOptionalParameter("bt_max_upload", maxUploadSpeed?.ToString())}")
                .ToObservable()
                .ValidateAndParseResponse()
                .ToSignal();

        private static string GetOptionalParameter(string name, string value, bool escapeValue = true)
        {
            if (value == null)
            {
                return null;
            }

            value = escapeValue ? EscapeParameter(value) : value;

            return $"&{name}={value}";
        }

        private static string EscapeParameter(string value) =>
            Uri.EscapeDataString(value);
    }

    internal static class Extensions
    {
        public static IObservable<Unit> ToSignal<T>(this IObservable<T> @this) =>
            @this
                .Select(_ => Unit.Default);

        public static IObservable<JObject> ValidateAndParseResponse(this IObservable<HttpResponseMessage> @this) =>
            @this
                .SelectMany(
                    response =>
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException($"Request failed ({response.StatusCode}).");
                        }

                        return response.Content.ReadAsStreamAsync();
                    })
                // can't read as string because Syno are returning "UTF-8" (including quotes) instead of just "UTF-8" sans quotes, which makes the ReadAsStringAsync method fall over
                .SelectMany(stream => new StreamReader(stream, Encoding.UTF8).ReadToEndAsync())
                .Select(
                    responseString =>
                    {
                        var jobject = JObject.Parse(responseString);
                        var result = jobject["success"].Value<bool>();

                        if (!result)
                        {
                            var code = jobject["error"]?["code"];
                            throw new HttpRequestException($"Response indicated failure (code {code}).");
                        }

                        return jobject;
                    });
    }
}