using System;

namespace Confifu.Config.Yaml.Graph
{
    public sealed class ReferenceLoopException : Exception
    {
        public ReferenceLoopException(string msg) : base(msg) { }
    }
}