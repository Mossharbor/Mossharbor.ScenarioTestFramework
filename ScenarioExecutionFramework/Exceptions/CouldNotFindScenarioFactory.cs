using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{
    [Serializable]
    public class CouldNotFindScenarioFactoryException : Exception
    {
        public CouldNotFindScenarioFactoryException() { }
        public CouldNotFindScenarioFactoryException(string message) : base(message) { }
        public CouldNotFindScenarioFactoryException(string message, Exception inner) : base(message, inner) { }
        protected CouldNotFindScenarioFactoryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
