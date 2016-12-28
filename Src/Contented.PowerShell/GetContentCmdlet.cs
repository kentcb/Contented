namespace Contented.PowerShell
{
    using System.Linq;
    using System.Management.Automation;
    using System.Reactive.Linq;

    [Cmdlet(VerbsCommon.Get, "Content")]
    public class GetContentCmdlet : ContentedCmdlet
    {
        protected override void ProcessRecord()
        {
            var synologyApi = this.SynologyApi;
            var downloadDtos = synologyApi
                .Authenticate(this.Credentials.UserName, this.Credentials.Password)
                .SelectMany(_ => synologyApi.GetDownloadTasks())
                .Wait();

            foreach (var downloadDto in downloadDtos)
            {
                var download = new Download(this.Credentials, downloadDto);
                this.WriteObject(download);
            }
        }
    }
}