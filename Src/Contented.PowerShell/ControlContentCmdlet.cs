namespace Contented.PowerShell
{
    using System.Linq;
    using System.Management.Automation;
    using System.Reactive.Linq;

    [Cmdlet("Control", "Contented")]
    public sealed class ControlContentCmdlet : ContentedCmdlet
    {
        [Parameter(
            HelpMessage = "Maximum download speed.")]
        public int? MaxDownloadSpeed
        {
            get;
            set;
        }

        [Parameter(
            HelpMessage = "Maximum upload speed.")]
        public int? MaxUploadSpeed
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            var synologyApi = this.SynologyApi;
            var results = synologyApi
                .Authenticate(this.Credentials.UserName, this.Credentials.Password)
                .SelectMany(_ => synologyApi.Configure(this.MaxDownloadSpeed, this.MaxUploadSpeed))
                .Wait();
        }
    }
}