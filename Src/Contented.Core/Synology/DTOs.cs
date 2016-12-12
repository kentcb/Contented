namespace Contented.Core.Synology
{
    using Newtonsoft.Json;

    public sealed class DownloadDto
    {
        [JsonProperty("additional")]
        public AdditionalDto Additional { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }
    }

    public sealed class AdditionalDto
    {
        [JsonProperty("detail")]
        public DetailDto Detail { get; set; }

        [JsonProperty("transfer")]
        public TransferDto Transfer { get; set; }
    }

    public sealed class DetailDto
    {
        [JsonProperty("connected_leechers")]
        public int ConnectedLeechers { get; set; }

        [JsonProperty("connected_seeders")]
        public int ConnectedSeeders { get; set; }

        [JsonProperty("create_time")]
        public long CreatedUnixTimestamp { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("total_peers")]
        public int TotalPeers { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }

    public sealed class TransferDto
    {
        [JsonProperty("size_downloaded")]
        public long Downloaded { get; set; }

        [JsonProperty("size_uploaded")]
        public long Uploaded { get; set; }

        [JsonProperty("speed_download")]
        public int DownloadSpeed { get; set; }

        [JsonProperty("speed_upload")]
        public int UploadSpeed { get; set; }
    }
}