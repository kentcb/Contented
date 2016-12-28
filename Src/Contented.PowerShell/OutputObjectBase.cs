namespace Contented.PowerShell
{
    using System.Management.Automation;

    public abstract class OutputObjectBase
    {
        private readonly PSCredential credentials;

        protected OutputObjectBase(
            PSCredential credentials)
        {
            this.credentials = credentials;
        }

        public PSCredential Credentials => this.credentials;
    }
}