namespace Contented.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Reactive.Linq;
    using Core;
    using Core.Synology;

    [Cmdlet(VerbsCommon.Add, "Content")]
    public sealed class AddContentCmdlet : ContentedCmdlet
    {
        [Parameter(
            Mandatory = true,
            HelpMessage = "Input content file.")]
        public string InputFile
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            var input = default(Input);

            using (var stream = new FileStream(this.InputFile, FileMode.Open))
            {
                input = Input.Parse(stream);
            }

            var synologyApi = this.SynologyApi;
            var results = synologyApi
                .Authenticate(this.Credentials.UserName, this.Credentials.Password)
                .SelectMany(_ => CreateDownloadTasks(synologyApi, input))
                .Wait();

            foreach (var result in results)
            {
                this.WriteObject(result);
            }
        }

        private IObservable<IImmutableList<AddedContent>> CreateDownloadTasks(SynologyApi synologyApi, Input input) =>
            input
                .Groups
                .SelectMany(group => group.Uris.Select(uri => new { Group = group, Uri = uri}))
                .Select(info => this.CreateDownloadTask(synologyApi, info.Group, info.Uri))
                .Merge()
                .Aggregate(
                    new List<AddedContent>(),
                    (acc, next) =>
                    {
                        acc.Add(next);
                        return acc;
                    },
                    results => results.ToImmutableList());

        private IObservable<AddedContent> CreateDownloadTask(SynologyApi synologyApi, InputGroup group, string uri) =>
            synologyApi
                .CreateFolder(group.TargetParent, group.TargetName, true)
                .Select(
                    _ =>
                        synologyApi
                            .CreateDownloadTask(Uri.EscapeDataString(uri), group.Target)
                            .Select(__ => new AddedContent(this.Credentials, uri, group.Target)))
                .Switch();
    }
}