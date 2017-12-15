using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    public abstract class QuickFactory : IScenarioFactory
    {
        public string Assembly { get; set; }
        public string FactoryName { get; set; }
        public IEnumerable<string> ScenarioList { get { return ScenarioMap.Keys; } }
        public IScenario[] GetScenarios(string scenarioName)
        {
            List<IScenario> scenarios = new List<IScenario>();
            if (ScenarioMap.ContainsKey(scenarioName))
            {
                IScenario scen = ScenarioMap[scenarioName]();
                scen.Assembly = this.Assembly;
                scen.Factory = this.FactoryName;
                scen.Name = scenarioName;
                scenarios.Add(scen);
            }

            return scenarios.ToArray();
        }

        public abstract Dictionary<string, Func<IScenario>> ScenarioMap { get; }
    }
}
