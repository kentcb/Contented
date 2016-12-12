namespace Contented.Core
{
    using System;

    public sealed class InputException : Exception
    {
        public InputException(string message)
            : base(message)
        {
        }
    }
}