using System;

namespace Confifu.Config.Yaml.Graph
{
    internal static class PathBuilder
    {
        private const char Separator = ':';

        public static string BuildSuffixPath(MappingNode node, string suffixPath)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (suffixPath == null) throw new ArgumentNullException(nameof(suffixPath));

            return node.Parent == null ? suffixPath : node.Name + Separator + suffixPath;
        }

        public static string BuildPrefixPath(string prefixPath, Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.Parent != null && prefixPath == null) throw new ArgumentException("prefixPath should not be null if node.Parent is not null", nameof(prefixPath));

            if (node.Parent == null) return string.Empty;
            return string.IsNullOrEmpty(prefixPath) ? node.Name : prefixPath + Separator + node.Name;
        }
    }
}