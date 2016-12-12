namespace Contented.Core
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public sealed class Input
    {
        private readonly IImmutableList<InputGroup> groups;

        public Input(
            IImmutableList<InputGroup> groups)
        {
            this.groups = groups;
        }

        public IImmutableList<InputGroup> Groups => this.groups;

        public static Input Parse(Stream stream)
        {
            var groups = new List<InputGroup>();

            using (var reader = new StreamReader(stream))
            {
                var group = default(InputGroup);

                while ((group = InputGroup.Parse(reader)) != null)
                {
                    groups.Add(group);
                }

                if (groups.Count == 0)
                {
                    throw new InputException("Could not find any groups.");
                }

                return new Input(
                    groups.ToImmutableList());
            }
        }
    }

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