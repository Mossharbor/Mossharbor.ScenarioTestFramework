using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework.Exceptions
{
    [Serializable]
    public class CannotChangeLogDirectoryAfterLoggerHasStartedException : Exception
    {
        public CannotChangeLogDirectoryAfterLoggerHasStartedException() { }
        public CannotChangeLogDirectoryAfterLoggerHasStartedException(string message) : base(message) { }
        public CannotChangeLogDirectoryAfterLoggerHasStartedException(string message, Exception inner) : base(message, inner) { }
        protected CannotChangeLogDirectoryAfterLoggerHasStartedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
