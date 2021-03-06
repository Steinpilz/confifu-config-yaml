﻿using System.Diagnostics;

namespace Confifu.Config.Yaml.Graph
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    internal abstract class Node
    {
        // keep 'tree' generated by traversal of GraphExtractor
        public Node Parent { get; internal set; }
        public string Name { get; internal set; }
    }
}