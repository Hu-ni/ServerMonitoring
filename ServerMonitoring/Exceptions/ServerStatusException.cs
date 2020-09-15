using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMonitoring.Exceptions
{
    [System.Serializable]
    public class ServerStatusException : System.Exception
    {
        public ServerStatusException() { }
        public ServerStatusException(string message) : base(message) { }
        public ServerStatusException(string message, System.Exception inner) : base(message, inner) { }
        protected ServerStatusException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
