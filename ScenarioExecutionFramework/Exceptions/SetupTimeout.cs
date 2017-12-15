using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework.Exceptions
{
    [Serializable]
    public class SetupTimeoutExceptionException : Exception
    {
        public SetupTimeoutExceptionException() { }
        public SetupTimeoutExceptionException(string message) : base(message) { }
        public SetupTimeoutExceptionException(string message, Exception inner) : base(message, inner) { }
        protected SetupTimeoutExceptionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
