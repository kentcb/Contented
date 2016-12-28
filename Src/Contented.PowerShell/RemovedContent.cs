namespace Contented.PowerShell
{
    using System.Management.Automation;

    public sealed class RemovedContent : OutputObjectBase
    {
        private readonly string id;

        public RemovedContent(
            PSCredential credentials,
            string id)
            : base(credentials)
        {
            this.id = id;
        }

        public string Id => this.id;
    }
}