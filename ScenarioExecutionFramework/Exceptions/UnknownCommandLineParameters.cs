using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{
    [Serializable]
    public class UnknownCommandLineParametersException : Exception
    {
        public UnknownCommandLineParametersException() { }
        public UnknownCommandLineParametersException(string message) : base(message) { }
        public UnknownCommandLineParametersException(string message, Exception inner) : base(message, inner) { }
        protected UnknownCommandLineParametersException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
