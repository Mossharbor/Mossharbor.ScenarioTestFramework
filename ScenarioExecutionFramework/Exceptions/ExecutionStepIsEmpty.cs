using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{

    [Serializable]
    public class ExecutionStepIsEmptyException : Exception
    {
        public ExecutionStepIsEmptyException() { }
        public ExecutionStepIsEmptyException(string message) : base(message) { }
        public ExecutionStepIsEmptyException(string message, Exception inner) : base(message, inner) { }
        protected ExecutionStepIsEmptyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
