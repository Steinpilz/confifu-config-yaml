using System;
using System.Collections.Generic;

namespace Confifu.Config.Yaml
{
    internal static class ConfigBuilder
    {
        private const string ConfigSeparator = ":";
        private const string InjectOperatorKey = "<<";

        private static IDictionary<object, object> AsDictionary(object obj) => obj as IDictionary<object, object>;

        private static string AsString(object obj) => obj as string;

        private static string GetPath(string path, string key)
        {
            if (key == InjectOperatorKey) return path;
            return path + (string.IsNullOrEmpty(path)
                       ? string.Empty
                       : ConfigSeparator) + key;
        }


        private static void Visit(object obj, string path, IDictionary<string, string> cfg)
        {
            var dict = AsDictionary(obj);
            if (dict == null)
            {
                var str = AsString(obj);
                if (str != null) cfg[path] = str;
                return;
            }

            foreach (var kvp in dict)
            {
                var key = AsString(kvp.Key);
                if (key == null) continue;

                var valuePath = GetPath(path, key);
                Visit(kvp.Value, valuePath, cfg);
            }
        }

        private static void Visit(object obj, IDictionary<string, string> cfg) => Visit(obj, string.Empty, cfg);

        public static Dictionary<string, string> Build(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var cfg = new Dictionary<string, string>();
            Visit(obj, cfg);
            return cfg;
        }
    }
}