namespace Contented.PowerShell
{
    using System;
    using System.Management.Automation;
    using Contented.Core.Synology;
    using Humanizer;

    public sealed class Download : OutputObjectBase
    {
        private readonly string id;
        private readonly long size;
        private readonly string status;
        private readonly string title;
        private readonly string type;
        private readonly string userName;
        private readonly int connectedLeechers;
        private readonly int connectedSeeders;
        private readonly DateTime created;
        private readonly string destination;
        private readonly string priority;
        private readonly int totalPeers;
        private readonly string uri;
        private readonly long downloaded;
        private readonly long uploaded;
        private readonly int downloadSpeed;
        private readonly int uploadSpeed;

        public Download(
            PSCredential credentials,
            DownloadDto downloadDto)
            : base(credentials)
        {
            this.id = downloadDto.Id;
            this.size = downloadDto.Size;
            this.status = downloadDto.Status;
            this.title = downloadDto.Title;
            this.type = downloadDto.Type;
            this.userName = downloadDto.UserName;
            this.connectedLeechers = downloadDto.Additional.Detail.ConnectedLeechers;
            this.connectedSeeders = downloadDto.Additional.Detail.ConnectedSeeders;
            this.created = downloadDto.Additional.Detail.CreatedUnixTimestamp.ToDateTime().ToLocalTime();
            this.destination = downloadDto.Additional.Detail.Destination;
            this.priority = downloadDto.Additional.Detail.Priority;
            this.totalPeers = downloadDto.Additional.Detail.TotalPeers;
            this.uri = downloadDto.Additional.Detail.Uri;
            this.downloaded = downloadDto.Additional.Transfer.Downloaded;
            this.uploaded = downloadDto.Additional.Transfer.Uploaded;
            this.downloadSpeed = downloadDto.Additional.Transfer.DownloadSpeed;
            this.uploadSpeed = downloadDto.Additional.Transfer.UploadSpeed;
        }

        public string Id => this.id;

        public long Size => this.size;

        public string SizeDisplay => this.size == 0 ? "Unknown" : this.size.Bytes().Humanize("0.#");

        public string Status => this.status;

        public string Title => this.title;

        public string Type => this.type;

        public string UserName => this.userName;

        public int ConnectedLeechers => this.connectedLeechers;

        public int ConnectedSeeders => this.connectedSeeders;

        public DateTime Created => this.created;

        public string CreatedDisplay => this.created.Humanize(utcDate: false);

        public string Destination => this.destination;

        public string Priority => this.priority;

        public int TotalPeers => this.totalPeers;

        public string Uri => this.uri;

        public long Downloaded => this.downloaded;

        public string DownloadedDisplay => this.downloaded.Bytes().Humanize("0.#");

        public long Uploaded => this.uploaded;

        public string UploadedDisplay => this.uploaded.Bytes().Humanize("0.#");

        public double? Ratio
        {
            get
            {
                if (this.uploaded == 0 || this.downloaded == 0)
                {
                    return null;
                }

                return (double)this.uploaded / this.downloaded;
            }
        }

        public string RatioDisplay => this.Ratio?.ToString("0.00") ?? "-";

        public int DownloadSpeed => this.downloadSpeed;

        public string DownloadSpeedDisplay => this.downloadSpeed.Bytes().Humanize("0.#") + "/s";

        public int UploadSpeed => this.uploadSpeed;

        public string UploadSpeedDisplay => this.uploadSpeed.Bytes().Humanize("0.#") + "/s";
    }
}