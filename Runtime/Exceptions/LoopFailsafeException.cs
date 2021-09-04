using System;

namespace Acedia
{

    // TODO LoopFailsafe: Example Code
    public class LoopFailsafeException : Exception
    {
        public LoopFailsafeException() { }
        public LoopFailsafeException(string message) : base(message) { }
        public LoopFailsafeException(string message, Exception inner) : base(message, inner) { }
    }
}
