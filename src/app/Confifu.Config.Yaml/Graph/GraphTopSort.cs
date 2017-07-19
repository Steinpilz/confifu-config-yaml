using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu.Config.Yaml.Graph
{
    internal static class GraphTopSort
    {
        private static IEnumerable<Node> Sort(Node node, ISet<Node> visited)
        {
            if (node == null || !visited.Add(node)) return Enumerable.Empty<Node>();

            if (node is ScalarNode scalar) return new[] {scalar};
            if (!(node is MappingNode mapping)) throw new ArgumentException("Unsupported node.", nameof(node));

            var dependencies = mapping.Children.Select(x => x.Value).Concat(mapping.Merges);
            return dependencies.SelectMany(x => Sort(x, visited)).Concat(new[] {node});
        }

        public static IEnumerable<Node> Sort(Node root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            return Sort(root, new HashSet<Node>());
        }
    }
}