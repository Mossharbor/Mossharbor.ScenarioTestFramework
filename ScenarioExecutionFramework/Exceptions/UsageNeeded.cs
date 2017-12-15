using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework.Exceptions
{
    [Serializable]
    public class UsageNeededException : Exception
    {
        public UsageNeededException() { }
        public UsageNeededException(string message) : base(message) { }
        public UsageNeededException(string message, Exception inner) : base(message, inner) { }
        protected UsageNeededException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
