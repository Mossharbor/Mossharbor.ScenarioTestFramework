using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework.Exceptions
{
    [Serializable]
    public class CanLogResultWhenScenarioIsNotRunningException : Exception
    {
        public CanLogResultWhenScenarioIsNotRunningException() { }
        public CanLogResultWhenScenarioIsNotRunningException(string message) : base(message) { }
        public CanLogResultWhenScenarioIsNotRunningException(string message, Exception inner) : base(message, inner) { }
        protected CanLogResultWhenScenarioIsNotRunningException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
