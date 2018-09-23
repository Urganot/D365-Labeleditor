using System;

namespace AVA_LabelEditor.CustomExceptions
{
    [Serializable]
    class ConnectionAlreadyOpenException : Exception
    {
        public ConnectionAlreadyOpenException(string message) : base(message)
        {
        }
    }
}
