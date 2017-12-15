using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionAssistant
{
    using ScenarioExecutionFramework;

    class Program
    {
        static void Main(string[] args)
        {
            ScenarioExecutionFramework.RuntimeParameters.Instance.Parse(args);

            string assembly = RuntimeParameters.Instance.AssemblyContainingFactory;
            string factory = RuntimeParameters.Instance.FactoryContainingScenario;
            string scenarioName = RuntimeParameters.Instance.ScenarioNameToRun;

            if (!String.IsNullOrEmpty(factory) && !String.IsNullOrEmpty(assembly))
                ScenarioManager.Instance.LoadFactory(assembly, factory);
            else if (!String.IsNullOrEmpty(assembly))
                ScenarioManager.Instance.LoadFactory(assembly);

            //TODO check for scenarioName is null or not

            ScenarioManager.Instance.LoadScenario(scenarioName);
            ScenarioManager.Instance.ExecuteScenarios();
        }
    }
}
