using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework.Exceptions
{
    [Serializable]
    public class CannotLoadScenarioFactoryException : Exception
    {
        public CannotLoadScenarioFactoryException() { }
        public CannotLoadScenarioFactoryException(string message) : base(message) { }
        public CannotLoadScenarioFactoryException(string message, Exception inner) : base(message, inner) { }
        protected CannotLoadScenarioFactoryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
