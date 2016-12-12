namespace Contented.PowerShell
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    public static class SecureStringExtensions
    {
        public static string ToUnsecureString(this SecureString @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            var unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = SecureStringMarshal.SecureStringToGlobalAllocUnicode(@this);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}