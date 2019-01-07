using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionTests
{
    using Mossharbor.ScenarioTestFramework;
    public class BasicCustomFactory : IScenarioFactory
    {
        public string Assembly { get; set; }
        public string FactoryName { get; set; }

        Dictionary<string, Func<IScenario>> scenarios = new Dictionary<string, Func<IScenario>>
        {
            {"VerifyBasicAdd",()=>
                {
                    return new Scenario("VerifyBasicAdd", "ExecutionTests", "", null, () =>
                        {
                            Logger.Instance.WriteLine("Hello VerifyBasicAdd");
                        }, null, null);
                }
            },
            {"VerifyAddBasicSubScenario",()=>
                {
                    Scenario v = new Scenario("VerifyBasicAdd2", "ExecutionTests", "", null, () =>
                        {
                            Logger.Instance.WriteLine("Hello");
                        }, null, null);

                    v.Add(() => { Logger.Instance.WriteLine("There VerifyAddBasicSubScenario"); });
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
                scen.Assembly = "ExecutionTests";
                scen.Name = scenarioName;
                return new IScenario[] { scen };
            }

            return null;
        }
    }
}
