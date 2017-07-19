using System;

namespace Confifu.Config.Yaml.Graph
{
    internal sealed class ScalarNode : Node
    {
        public string Value { get; }

        public ScalarNode(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}