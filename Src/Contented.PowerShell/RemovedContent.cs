namespace Contented.PowerShell
{
    using System.Security;

    public sealed class RemovedContent : OutputObjectBase
    {
        private readonly string id;

        public RemovedContent(
            string user,
            SecureString password,
            string id)
            : base(user, password)
        {
            this.id = id;
        }

        public string Id => this.id;
    }
}