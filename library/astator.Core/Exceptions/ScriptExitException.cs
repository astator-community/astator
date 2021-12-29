using System;

namespace astator.Core.Exceptions
{
    public class ScriptExitException : Exception
    {
        public ScriptExitException(string message) : base(message)
        {

        }
    }
}
