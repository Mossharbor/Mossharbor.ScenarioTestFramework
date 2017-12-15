using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    public interface IScenarioFactory
    {
        string Assembly { get; set; }
        string FactoryName { get; set; }
        IEnumerable<string> ScenarioList { get; }
        IScenario[] GetScenarios(string scenarioName);
    }
}
