using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{
    [Serializable]
    public class CannotLoadScenarioAssemblyException : Exception
    {
        public CannotLoadScenarioAssemblyException() { }
        public CannotLoadScenarioAssemblyException(string message) : base(message) { }
        public CannotLoadScenarioAssemblyException(string message, Exception inner) : base(message, inner) { }
        protected CannotLoadScenarioAssemblyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
