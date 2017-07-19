using System.Collections.Generic;

namespace Confifu.Config.Yaml.Graph
{
    internal sealed class PathResolver
    {
        private readonly Dictionary<Node, string> _pathes = new Dictionary<Node, string>();

        public string GetPath(Node node)
        {
            if (node == null) return null;

            if (_pathes.TryGetValue(node, out string path)) return path;
            var parentPath = GetPath(node.Parent);
            path = PathBuilder.BuildPrefixPath(parentPath, node);
            return _pathes[node] = path;
        }
    }
}
