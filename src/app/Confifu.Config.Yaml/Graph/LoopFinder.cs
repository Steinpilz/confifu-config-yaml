using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu.Config.Yaml.Graph
{
    internal static class LoopFinder
    {
        private static List<Node> FindLoop(Node node, IDictionary<Node, NodeState> states)
        {
            if (!(node is MappingNode mapping)) return null;

            states[node] = NodeState.Visiting;

            var dependencies = mapping.Children.Values.Concat(mapping.Merges);
            foreach (var dependency in dependencies)
            {
                var isChildHasState = states.TryGetValue(dependency, out NodeState childState);
                if (isChildHasState)
                {
                    if (childState == NodeState.Visiting) return CreateLoop(node, dependency);

                    continue; // don't visit Visited node
                }

                var loop = FindLoop(dependency, states);
                if (loop != null) return AddNodeToLoop(loop, node);
            }

            states[node] = NodeState.Visited;

            return null;
        }

        private static List<Node> CreateLoop(Node node, Node child)
        {
            var loop = new List<Node> { child };
            if (node != child) loop.Add(node);
            return loop;
        }

        private static List<Node> AddNodeToLoop(List<Node> loop, Node node)
        {
            if (loop == null) return new List<Node> { node };

            // null is mark that loop is fully placed in result, no more nodes needed
            if (loop.Last() == null) return loop;

            loop.Add(node);

            // bad child had Visiting state, so it should be in result 2 times
            if (loop.First() == node) loop.Add(null);

            return loop;
        }


        public static IEnumerable<Node> Find(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            var states = new Dictionary<Node, NodeState>();

            var loop = FindLoop(node, states);
            if (loop == null) return null;
            loop.RemoveAt(loop.Count - 1); // remove null mark
            loop.Reverse();
            return loop;
        }
    }
}