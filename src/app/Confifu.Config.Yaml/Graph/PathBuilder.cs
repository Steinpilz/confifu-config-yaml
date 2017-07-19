using System;

namespace Confifu.Config.Yaml.Graph
{
    internal static class PathBuilder
    {
        private const char Separator = ':';

        public static string GetSuffixPath(MappingNode node, string forwardPath)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (forwardPath == null) throw new ArgumentNullException(nameof(forwardPath));

            return node.Parent == null ? forwardPath : node.Name + Separator + forwardPath;
        }

        public static string GetPrefixPath(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return node.Parent == null ? string.Empty : GetPrefixPath(string.Empty, node);
        }

        public static string GetPrefixPath(string parentPath, Node node)
        {
            if (parentPath == null) throw new ArgumentNullException(nameof(parentPath));
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.Parent == null) throw new ArgumentException("Node should have parent.");

            return string.IsNullOrEmpty(parentPath) ? node.Name : parentPath + Separator + node.Name;
        }
    }
}