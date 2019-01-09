using System;
using System.Runtime.Serialization;

namespace AVA_LabelEditor.CustomExceptions
{
    [Serializable]
    internal class ConnectionAlreadyClosedException : Exception
    {
        public ConnectionAlreadyClosedException()
        {
        }

        public ConnectionAlreadyClosedException(string message) : base(message)
        {
        }

        public ConnectionAlreadyClosedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConnectionAlreadyClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}