using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    [Serializable]
    public class CouldNotFindScenarioFactoryAssemblyException : Exception
    {
        public CouldNotFindScenarioFactoryAssemblyException() { }
        public CouldNotFindScenarioFactoryAssemblyException(string message) : base(message) { }
        public CouldNotFindScenarioFactoryAssemblyException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotFindScenarioFactoryAssemblyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
