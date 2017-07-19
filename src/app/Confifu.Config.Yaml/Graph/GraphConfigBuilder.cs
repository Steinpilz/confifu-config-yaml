using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu.Config.Yaml.Graph
{
    // traverse nodes in top sort order, make config for each (because of top sort dependencies are resolved), use it for merging dependent nodes
    internal static class GraphConfigBuilder
    {
        private static void Insert(IEnumerable<KeyValuePair<string, string>> source, IDictionary<string, string> destination)
        {
            foreach (var kvp in source)
            {
                destination[kvp.Key] = kvp.Value;
            }
        }

        private static void HandleMapping(MappingNode node, IDictionary<Node, Dictionary<string, string>> cfgs)
        {
            var nodeConfig = new Dictionary<string, string>();
            var children = node.Children;
            

            // go through all merge dependencies and insert them
            var insertMappings = node.Merges;

            foreach (var mapping in insertMappings)
            {
                var cfgToInsert = cfgs[mapping];
                Insert(cfgToInsert, nodeConfig);
            }


            // then go through remainder dependencies
            var otherMappings = children.Where(x => x.Value is MappingNode);

            foreach (var otherMappingsKvp in otherMappings)
            {
                var mapping = (MappingNode) otherMappingsKvp.Value;

                var cfgToInsert = cfgs[mapping];
                var modifiedCfgToInsert = cfgToInsert.Select(kvp => new KeyValuePair<string, string>(PathBuilder.BuildSuffixPath(mapping, kvp.Key), kvp.Value));
                Insert(modifiedCfgToInsert, nodeConfig);
            }

            // then go though scalars
            var scalarsKvps = children.Where(x => x.Value is ScalarNode);
            foreach (var scalarKvp in scalarsKvps)
            {
                var scalar = (ScalarNode) scalarKvp.Value;
                nodeConfig[scalarKvp.Key] = scalar.Value;
            }

            cfgs.Add(node, nodeConfig);
        }


        public static Dictionary<string, string> Build(Node root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));

            var cfgs = new Dictionary<Node, Dictionary<string, string>>();
            var mappings = GraphTopSort.Sort(root).OfType<MappingNode>();
            foreach (var mapping in mappings)
            {
                HandleMapping(mapping, cfgs);
            }

            return cfgs[root];
        }
    }
}