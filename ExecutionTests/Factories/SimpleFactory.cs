using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionTests
{
    using ScenarioExecutionFramework;

    class SimpleTestFactory : SimpleFactory
    {
        public override IEnumerable<string> ScenarioList
        {
            get {return new List<string>{
                    "VerifyBasicAdd"
                    ,"VerifyAddBasicSubScenario"
                    ,"TwoScenarios"
                };
            }
        }

        protected override void ModifyScenarioWith(string scenarioName)
        {
            switch (scenarioName)
            {
                case "TwoScenarios":
                    AddScenario("VerifyBasicAdd");
                    AddScenario("VerifyBasicAdd", true);
                    break;
                case "VerifyBasicAdd":
                    this.Add(() => { Logger.Instance.WriteLine("Hello VerifyBasicAdd"); });
                    break;

                case "VerifyAddBasicSubScenario":
                    this.Add(() => { Logger.Instance.WriteLine("Hello VerifyBasicAdd"); });
                    this.Add(() => { Logger.Instance.WriteLine("There VerifyAddBasicSubScenario"); });
                    break;
            }
        }
    }
}
