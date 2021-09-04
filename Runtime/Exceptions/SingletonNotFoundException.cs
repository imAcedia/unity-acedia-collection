using System;

namespace Acedia
{
    public class SingletonNotFoundException : Exception
    {
        public Type type { get; private set; }

        // LCONSIDER SingletonNotFoundException: Better phrasing?
        public SingletonNotFoundException(Type type) : base($"{type} instance not found or has not been initialized.")
            => this.type = type;
        public SingletonNotFoundException(Type type, string message) : base(message) => this.type = type;
        public SingletonNotFoundException(Type type, string message, Exception inner) : base(message, inner) => this.type = type;
    }
}
