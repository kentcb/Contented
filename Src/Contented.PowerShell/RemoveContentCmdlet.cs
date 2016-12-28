namespace Contented.PowerShell
{
    using System.Collections.Immutable;
    using System.Management.Automation;
    using System.Reactive.Linq;

    [Cmdlet(VerbsCommon.Remove, "Content")]
    public sealed class RemoveContentCmdlet : ContentedCmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "Id of content to be removed.",
            ValueFromPipelineByPropertyName = true)]
        public string Id
        {
            get;
            set;
        }

        [Parameter(
            HelpMessage = "Optionally force uncompleted download files to be copied to the destination.")]
        public bool ForceComplete
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            var synologyApi = this.SynologyApi;
            var removedIds = synologyApi
                .Authenticate(this.Credentials.UserName, this.Credentials.Password)
                .SelectMany(_ => synologyApi.RemoveDownloadTasks(new[] { this.Id }.ToImmutableList(), this.ForceComplete))
                .Wait();

            foreach (var removedId in removedIds)
            {
                var removedContent = new RemovedContent(this.Credentials, removedId);
                this.WriteObject(removedContent);
            }
        }
    }
}