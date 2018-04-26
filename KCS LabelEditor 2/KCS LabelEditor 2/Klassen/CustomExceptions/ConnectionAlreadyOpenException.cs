using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCS_LabelEditor_2.Klassen.CustomExceptions
{
    class ConnectionAlreadyOpenException : Exception
    {
        public ConnectionAlreadyOpenException(string message) : base(message)
        {
        }
    }
}
