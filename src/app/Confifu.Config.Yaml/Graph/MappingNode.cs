using System;
using System.Collections.Generic;

namespace Confifu.Config.Yaml.Graph
{
    internal sealed class MappingNode : Node
    {
        public IReadOnlyDictionary<string, Node> Children { get;}
        public IReadOnlyList<MappingNode> Merges { get; }

        public MappingNode(IReadOnlyDictionary<string, Node> children, IReadOnlyList<MappingNode> merges)
        {
            Children = children ?? throw new ArgumentNullException(nameof(children));
            Merges = merges ?? throw new ArgumentNullException(nameof(merges));
        }
    }
}
