using System;

namespace AVA_LabelEditor.CustomExceptions
{
    class ConnectionAlreadyOpenException : Exception
    {
        public ConnectionAlreadyOpenException(string message) : base(message)
        {
        }
    }
}
