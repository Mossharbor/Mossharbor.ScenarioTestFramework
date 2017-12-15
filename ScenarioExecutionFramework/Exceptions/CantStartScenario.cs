using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework.Exceptions
{
    [Serializable]
    public class CantStartScenarioException : Exception
    {
        public CantStartScenarioException() { }
        public CantStartScenarioException(string message) : base(message) { }
        public CantStartScenarioException(string message, Exception inner) : base(message, inner) { }
        protected CantStartScenarioException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
