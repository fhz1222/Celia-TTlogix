using System;
using System.Runtime.Serialization;

namespace TT.Services.Label
{
    [Serializable]
    internal class PrinterUnavailableException : Exception
    {
        public PrinterUnavailableException()
        {
        }

        public PrinterUnavailableException(string message) : base(message)
        {
        }

        public PrinterUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PrinterUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}