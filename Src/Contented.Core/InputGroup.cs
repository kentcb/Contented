namespace Contented.Core
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    [DebuggerDisplay("{Target}")]
    public sealed class InputGroup
    {
        private readonly string target;
        private readonly IImmutableList<string> uris;

        public InputGroup(
            string target,
            IImmutableList<string> uris)
        {
            this.target = target;
            this.uris = uris;
        }

        public string Target => this.target;

        public string TargetParent
        {
            get
            {
                var pathParts = GetPathParts(this.target);
                return pathParts
                    .Take(pathParts.Count - 1)
                    .Aggregate("", (acc, next) => acc + "/" + next);
            }
        }

        public string TargetName => GetPathParts(this.target).LastOrDefault();

        public IImmutableList<string> Uris => this.uris;

        public static InputGroup Parse(StreamReader reader)
        {
            var line = default(string);
            var target = default(string);
            var uris = new List<string>();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                target = line;
                break;
            }

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                uris.Add(line);
            }

            if (target == null)
            {
                return null;
            }

            return new InputGroup(
                target,
                uris.ToImmutableList());
        }

        private static IImmutableList<string> GetPathParts(string path) =>
            path
                .Split('/', '\\')
                .ToImmutableList();
    }
}