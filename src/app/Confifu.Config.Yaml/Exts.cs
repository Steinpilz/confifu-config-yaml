using System;
using Confifu.Abstractions;

namespace Confifu.Config.Yaml
{
    public static class Exts
    {
        public static ConfigVariablesBuilder Yaml(
            this ConfigVariablesBuilder builder,
            Action<YamlConfigVariablesBuilder> configAction
        )
        {
            var yamlBuilder = new YamlConfigVariablesBuilder();

            configAction?.Invoke(yamlBuilder);
            builder.AddBuilder(yamlBuilder);

            return builder;
        }

        public static ConfigVariablesBuilder Yaml(this ConfigVariablesBuilder builder,
            string yamlContent)
        {

            return builder.Yaml(b => b.UseYamlContent(yamlContent));
        }

        public static ConfigVariablesBuilder YamlFile(this ConfigVariablesBuilder builder,
            string filePath, bool optional = false)
        {

            return builder.Yaml(b => b.UseFile(filePath, optional));
        }
    }
}
