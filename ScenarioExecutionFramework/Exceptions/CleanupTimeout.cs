using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework.Exceptions
{
    [Serializable]
    public class CleanupTimeoutException : Exception
    {
        public CleanupTimeoutException() { }
        public CleanupTimeoutException(string message) : base(message) { }
        public CleanupTimeoutException(string message, Exception inner) : base(message, inner) { }
        protected CleanupTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
