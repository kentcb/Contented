// TODO: can replace with Genesis.Join once that supports netstandard
namespace System.Linq
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="Join"/> extension method.
    /// </summary>
    public static class JoinExtension
    {
        /// <summary>
        /// Joins strings in the provided enumerable, separating them with <paramref name="separator"/>.
        /// </summary>
        /// <param name="this">
        /// The enumerable containing the strings to join.
        /// </param>
        /// <param name="separator">
        /// The separator to insert between consecutive strings.
        /// </param>
        /// <returns>
        /// A <c>string</c> containing all the provided strings separated by <paramref name="separator"/>.
        /// </returns>
        public static string Join(this IEnumerable<string> @this, string separator) =>
            string.Join(separator, @this);
    }
}