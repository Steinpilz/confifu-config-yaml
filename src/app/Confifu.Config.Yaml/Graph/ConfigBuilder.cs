using System;
using System.Collections.Generic;
using System.Linq;

namespace Confifu.Config.Yaml.Graph
{
    internal static class ConfigBuilder
    {
        public static Dictionary<string, string> Build(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var root = GraphExtractor.Extract(obj);
            var loop = LoopChecker.Check(root);
            if (loop == null) return GraphConfigBuilder.Build(root);

            var pathResolver = new PathResolver();
            var loopStr = string.Join(", ", loop.Select(x => $"'{pathResolver.GetPath(x)}'"));
            throw new ReferenceLoopException($"Loop detected: {loopStr}");
        }
    }
}