using System;
using System.Collections.Generic;

namespace Confifu.Config.Yaml.Graph
{
    internal sealed class PathResolver
    {
        private readonly Dictionary<Node, string> _pathes = new Dictionary<Node, string>();

        public string GetPath(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            if (_pathes.TryGetValue(node, out string path)) return path;
            var parent = node.Parent;
            path = parent == null ? PathBuilder.GetPrefixPath(node) : PathBuilder.GetPrefixPath(GetPath(parent), node);
            return _pathes[node] = path;
        }
    }
}
