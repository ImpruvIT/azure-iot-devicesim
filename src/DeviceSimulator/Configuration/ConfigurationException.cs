using System;
using System.Runtime.Serialization;

namespace ImpruvIT.Azure.IoT.DeviceSimulator.Configuration
{
    public class ConfigurationException : ApplicationException
    {
        public ConfigurationException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
        
        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}