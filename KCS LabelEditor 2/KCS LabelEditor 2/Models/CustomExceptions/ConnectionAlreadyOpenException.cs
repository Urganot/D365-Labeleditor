using System;

namespace KCS_LabelEditor_2.CustomExceptions
{
    class ConnectionAlreadyOpenException : Exception
    {
        public ConnectionAlreadyOpenException(string message) : base(message)
        {
        }
    }
}
