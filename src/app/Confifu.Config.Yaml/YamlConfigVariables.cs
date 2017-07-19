using System;
using System.Collections.Generic;
using System.IO;
using Confifu.Abstractions;
using Confifu.Config.Yaml.Graph;
using YamlDotNet.Serialization;

namespace Confifu.Config.Yaml
{
    public sealed class YamlConfigVariables : IConfigVariables
    {
        private readonly Dictionary<string, string> _dict;

        public YamlConfigVariables(string yamlContent)
        {
            if (yamlContent == null) throw new ArgumentNullException(nameof(yamlContent));

            _dict = Load(yamlContent);
        }


        private static Dictionary<string, string> Load(string yamlContent)
        {
            var stringReader = new StringReader(yamlContent);
            var deserializer = new Deserializer();
            var obj = deserializer.Deserialize(stringReader);
            return new Dictionary<string, string>(ConfigBuilder.Build(obj));
        }

        public string this[string key] => _dict.TryGetValue(key, out string result) ? result : null;
    }
}