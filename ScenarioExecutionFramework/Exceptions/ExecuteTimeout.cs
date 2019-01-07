using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{
    [Serializable]
    public class ExecuteTimeoutException : Exception
    {
        public ExecuteTimeoutException() { }
        public ExecuteTimeoutException(string message) : base(message) { }
        public ExecuteTimeoutException(string message, Exception inner) : base(message, inner) { }
        protected ExecuteTimeoutException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
