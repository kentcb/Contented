using System.Security;

namespace Contented.PowerShell
{
    public sealed class AddedContent : OutputObjectBase
    {
        private readonly string uri;
        private readonly string target;

        public AddedContent(
            string user,
            SecureString password,
            string uri,
            string target)
            : base(user, password)
        {
            this.uri = uri;
            this.target = target;
        }

        public string Uri => this.uri;

        public string Target => this.target;
    }
}