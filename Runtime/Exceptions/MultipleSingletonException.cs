using System;

namespace Acedia
{
    public class MultipleSingletonException : Exception
    {
        public Type type { get; private set; }

        // LCONSIDER MultipleSingletonException: Better phrasing?
        public MultipleSingletonException(Type type) : base($"Multiple {type} instance is found.")
            => this.type = type;
        public MultipleSingletonException(Type type, string message) : base(message) => this.type = type;
        public MultipleSingletonException(Type type, string message, Exception inner) : base(message, inner) => this.type = type;
    }
}
