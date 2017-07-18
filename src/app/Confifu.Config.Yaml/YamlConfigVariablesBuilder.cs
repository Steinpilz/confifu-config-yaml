using System;
using System.IO;
using Confifu.Abstractions;

namespace Confifu.Config.Yaml
{
    public class YamlConfigVariablesBuilder : IConfigVariablesBuilder
    {
        private IConfigVariables _result = new EmptyConfigVariables();

        public YamlConfigVariablesBuilder UseYamlContent(string yamlContent)
        {
            _result = new YamlConfigVariables(yamlContent);
            return this;
        }

        public YamlConfigVariablesBuilder UseFile(string filePath, bool optional)
        {
            if (!File.Exists(filePath))
            {
                if (!optional)
                    throw new InvalidOperationException($"Required file {filePath} not found");
                return this;
            }

            var fileContent = File.ReadAllText(filePath);
            _result = new YamlConfigVariables(fileContent);
            return this;
        }

        public IConfigVariables Build() => _result;
    }
}