namespace Contented.Core
{
    using System;

    public sealed class ContentedException : Exception
    {
        public ContentedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}