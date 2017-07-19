using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Confifu.Config.Yaml.Graph
{
    // traverse nodes in top sort order, make config for each (because of top sort dependencies are resolved), use it for merging dependent nodes
    internal static class GraphConfigBuilder
    {
        private static void HandleMerge(MappingNode node, IDictionary<Node, ImmutableDictionary<string, string>> cfgs)
        {
            var nodeConfig = ImmutableDictionary<string, string>.Empty;
            var children = node.Children;
            

            // go through all merge dependencies and pour them
            var pourMappings = node.Merges;

            foreach (var mapping in pourMappings)
            {
                var cfgToPour = cfgs[mapping];
                nodeConfig = nodeConfig.SetItems(cfgToPour);
            }


            // then go through remainder dependencies and rewrite nodeConfig
            var otherMappings = children.Where(x => x.Value is MappingNode);

            foreach (var otherMappingsKvp in otherMappings)
            {
                var mapping = (MappingNode) otherMappingsKvp.Value;

                var cfgToPour = cfgs[mapping];
                var modifiedCfg = cfgToPour.Select(kvp => new KeyValuePair<string, string>(PathBuilder.GetSuffixPath(mapping, kvp.Key), kvp.Value));
                nodeConfig = nodeConfig.SetItems(modifiedCfg);
            }

            // then go though scalars
            var cfg = new Dictionary<string, string>();
            var scalarsKvps = children.Where(x => x.Value is ScalarNode);
            foreach (var scalarKvp in scalarsKvps)
            {
                var scalar = (ScalarNode) scalarKvp.Value;
                cfg.Add(scalarKvp.Key, scalar.Value);
            }
            nodeConfig = nodeConfig.SetItems(cfg);

            cfgs.Add(node, nodeConfig);
        }


        public static ImmutableDictionary<string, string> Build(Node root)
        {
            var cfgs = new Dictionary<Node, ImmutableDictionary<string, string>>();
            var mappings = GraphTopSort.Sort(root).OfType<MappingNode>();
            foreach (var mapping in mappings)
            {
                HandleMerge(mapping, cfgs);
            }
            return cfgs[root];
        }
    }
}