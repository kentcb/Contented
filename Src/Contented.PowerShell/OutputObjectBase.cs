namespace Contented.PowerShell
{
    using System.Security;

    public abstract class OutputObjectBase
    {
        private readonly string user;
        private readonly SecureString password;

        protected OutputObjectBase(
            string user,
            SecureString password)
        {
            this.user = user;
            this.password = password;
        }

        public string User => this.user;

        public SecureString Password => this.password;
    }
}