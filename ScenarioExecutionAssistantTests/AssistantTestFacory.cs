using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionAssistantTests
{
    using Mossharbor.ScenarioTestFramework;
    public class AssistantTestFacory : IScenarioFactory
    {
        public string Assembly { get; set; }
        public string FactoryName { get; set; }

        Dictionary<string, Func<IScenario>> scenarios = new Dictionary<string, Func<IScenario>>
        {
            {"BasicTest",()=>
                {
                    return new Scenario("VerifyBasicAdd", "ScenarioExecutionAssistantTests", "", null, () =>
                        {
                            Logger.WriteLine("Hello VerifyBasicAdd");
                            Logger.LogPass("WePassed");
                        }, null, null);
                }
            },
            {"BasicSubScenario",()=>
                {
                    Scenario v = new Scenario("VerifyBasicAdd2", "ScenarioExecutionAssistantTests", "", null, () =>
                        {
                            Logger.WriteLine("Hello");
                            Logger.LogPass("WePassed");
                        }, null, null);

                    v.Add(() => { Logger.WriteLine("There VerifyAddBasicSubScenario"); });
                    return v;
                }
            }
        };
        public IEnumerable<string> ScenarioList { get { return scenarios.Keys; } }
        public IScenario[] GetScenarios(string scenarioName)
        {
            if (scenarios.ContainsKey(scenarioName))
            {
                IScenario scen = scenarios[scenarioName]();
                scen.Assembly = "ScenarioExecutionAssistantTests";
                scen.Name = scenarioName;
                return new IScenario[] { scen };
            }

            return null;
        }
    }
}
