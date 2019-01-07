using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{
    [Serializable]
    public class CannotFindScenarioAssemblyException : Exception
    {
        public CannotFindScenarioAssemblyException() { }
        public CannotFindScenarioAssemblyException(string message) : base(message) { }
        public CannotFindScenarioAssemblyException(string message, Exception inner) : base(message, inner) { }
        protected CannotFindScenarioAssemblyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
