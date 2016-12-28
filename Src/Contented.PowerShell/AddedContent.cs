namespace Contented.PowerShell
{
    using System.Management.Automation;

    public sealed class AddedContent : OutputObjectBase
    {
        private readonly string uri;
        private readonly string target;

        public AddedContent(
            PSCredential credentials,
            string uri,
            string target)
            : base(credentials)
        {
            this.uri = uri;
            this.target = target;
        }

        public string Uri => this.uri;

        public string Target => this.target;
    }
}