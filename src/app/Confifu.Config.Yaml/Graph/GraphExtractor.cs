using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu.Config.Yaml.Graph
{
    internal static class GraphExtractor
    {
        private const string MergeKey = "<<";


        private static IDictionary<object, object> AsMapping(object obj) => obj as IDictionary<object, object>;

        private static string AsScalar(object obj) => obj as string;

        private static bool IsMerge(string key) => key.StartsWith(MergeKey);


        private static MappingNode ExtractMapping(object obj, IDictionary<object, Node> nodes)
        {
            var dict = AsMapping(obj);
            if (dict == null) return null;

            var children = new Dictionary<string, Node>();
            var merges = new List<MappingNode>();
            var mergesKvps = new Dictionary<string, MappingNode>();
            var result = new MappingNode(children, merges);
            nodes.Add(obj, result);

            foreach (var kvp in dict)
            {
                var key = AsScalar(kvp.Key);
                if (key == null) throw new InvalidOperationException("Scalar Mapping key is supported only.");

                var isMerge = IsMerge(key);
                var name = isMerge ? null : key; // if ref, can not extract name

                var node = ExtractNode(kvp.Value, result, name, nodes);
                var mapping = node as MappingNode;
                if (isMerge && mapping == null) throw new InvalidOperationException("Merging operator should be used with Mappings only.");

                if (isMerge) mergesKvps.Add(key, mapping);
                else children.Add(key, node);
            }

            // to correctly process merges like <<1 <<2
            var orderedMerges = mergesKvps.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
            merges.AddRange(orderedMerges);

            return result;
        }

        private static ScalarNode ExtractScalar(object obj, IDictionary<object, Node> nodes)
        {
            var scal = AsScalar(obj);
            if (scal == null) return null;

            var result = new ScalarNode(scal);
            nodes.Add(obj, result);

            return result;
        }

        private static Node ExtractNode(object obj, IDictionary<object, Node> nodes)
        {
            if (nodes.TryGetValue(obj, out Node node)) return node;

            var mapping = ExtractMapping(obj, nodes);
            if (mapping != null) return mapping;

            var scalar = AsScalar(obj);
            if (scalar != null) return ExtractScalar(obj, nodes);

            throw new ArgumentException("Unsupported object.", nameof(obj));
        }

        private static Node ExtractNode(object obj, Node parent, string name, IDictionary<object, Node> nodes)
        {
            var node = ExtractNode(obj, nodes);
            if (name == null) return node;

            // we build tree on names, not refs
            node.Parent = parent;
            node.Name = name;
            return node;
        }


        public static Node Extract(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return ExtractNode(obj, null, string.Empty, new Dictionary<object, Node>());
        }
    }
}