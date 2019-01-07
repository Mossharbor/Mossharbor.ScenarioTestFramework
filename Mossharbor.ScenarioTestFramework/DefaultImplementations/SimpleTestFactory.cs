using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    public abstract class SimpleFactory : IScenarioFactory
    {
        int scenarioSteps = 1;
        Scenario currentScenario = null;
        protected Scenario CurrentScenario{get { return currentScenario; }}
        List<IScenario> scenarios = new List<IScenario>();

        public int ScenarioCount { get { return scenarios.Count; } }
        public int ScenarioSteps { get { return scenarioSteps; } }
        public string Assembly { get; set; }
        public string FactoryName { get; set; }
        public abstract IEnumerable<string> ScenarioList { get;  }
        public IScenario[] GetScenarios(string scenarioName)
        {
            scenarios.Clear();
            ModifyScenarioWith(scenarioName);
            ScenarioFinished();
            return scenarios.ToArray();
        }

        protected void Add(Action toExec)
        {
            ++scenarioSteps;
            if (null != currentScenario)
                this.currentScenario.Add(toExec);
            else
            {
                currentScenario = new Scenario(toExec);
                scenarios.Add(currentScenario);
            }
        }

        protected void Add(IScenarioStep step)
        {
            ++scenarioSteps;
            if (null != currentScenario)
                this.currentScenario.Add(() => step.SetUp(), () => step.ExecuteStep(), () => step.CleanUp(), () => step.EndRunCleanUp());
            else
            {
                currentScenario = new Scenario(() => step.SetUp(), () => step.ExecuteStep(), () => step.CleanUp(), ()=>step.EndRunCleanUp());
                scenarios.Add(currentScenario);
            }
        }

        protected void ScenarioFinished()
        {
            currentScenario = null;
        }
        protected void AddScenario(string scenarioName, bool treatAsNewScenario = true)
        {
            //if (firstCallToModifyScenarioWith)
            //{
            //    scenarios.Clear();
            //    firstCallToModifyScenarioWith = false;
            //}

            currentScenario = new Scenario(scenarioName);
            scenarios.Add(currentScenario);
            ModifyScenarioWith(scenarioName);
            ScenarioManager.Instance.ScenarioIds.Add(scenarioName);
        }
        public abstract void ModifyScenarioWith(string scenarioName);
    }
}
