using System;
using System.Runtime.Serialization;

namespace ToDo.Frontend.Services
{
    [Serializable]
    public class RestClientException : Exception
    {
        public RestClientException()
        {
        }

        public RestClientException(string message) 
            : base(message)
        {
        }

        public RestClientException(string message, Exception inner) 
            : base(message, inner)
        {
        }
        
        protected RestClientException(
            SerializationInfo info,
            StreamingContext context) 
            : base(info, context)
        {
        }
    }
}