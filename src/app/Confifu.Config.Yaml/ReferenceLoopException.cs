using System;

namespace Confifu.Config.Yaml
{
    public sealed class ReferenceLoopException : Exception
    {
        public ReferenceLoopException(string msg) : base(msg) { }
    }
}