namespace Contented.PowerShell
{
    using System;
    using System.Management.Automation;
    using System.Net.Http;
    using System.Net.Security;
    using System.Reactive.Linq;
    using System.Security;
    using System.Security.Cryptography.X509Certificates;
    using Core.Synology;

    public abstract class SynologyCmdlet : Cmdlet
    {
        private readonly Lazy<SynologyApi> synologyApi;

        protected SynologyCmdlet()
        {
            this.synologyApi = new Lazy<SynologyApi>(CreateSynologyApi);
        }

        [Parameter(
            Mandatory = true,
            HelpMessage = "Credentials for Synology access.",
            ValueFromPipelineByPropertyName = true)]
        public PSCredential Credentials
        {
            get;
            set;
        }

        protected SynologyApi SynologyApi => this.synologyApi.Value;

        private static SynologyApi CreateSynologyApi()
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = ValidateCertificate
            };
            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri("https://splinter:4001/webapi")
            };

            var synologyApi = new SynologyApi(httpClient);

            return synologyApi;
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (this.synologyApi.IsValueCreated)
            {
                this
                    .synologyApi
                    .Value
                    .LogOut()
                    .Wait();
            }
        }

        // don't think I can use Let's Encrypt until Syno 6, which doesn't run on my DS410 :(
        private static bool ValidateCertificate(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors errors) =>
            certificate.IssuerName.Name == "E=kent.boogaart@gmail.com, CN=KCB Consulting Ltd, OU=IT, O=KCB Consulting Ltd, L=Adelaide, S=South Australia, C=AU";
    }
}