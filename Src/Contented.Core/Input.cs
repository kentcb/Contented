namespace Contented.Core
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;

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
}