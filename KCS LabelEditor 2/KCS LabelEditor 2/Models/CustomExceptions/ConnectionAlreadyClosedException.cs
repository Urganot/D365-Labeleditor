using System;
using System.Runtime.Serialization;

namespace KCS_LabelEditor_2.CustomExceptions
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