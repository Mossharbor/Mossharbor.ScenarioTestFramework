﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionTests
{
    using Mossharbor.ScenarioTestFramework;

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

        public override void ModifyScenarioWith(string scenarioName)
        {
            switch (scenarioName)
            {
                case "TwoScenarios":
                    AddScenario("VerifyBasicAdd");
                    AddScenario("VerifyBasicAdd", true);
                    break;
                case "VerifyBasicAdd":
                    this.Add(() => { Logger.WriteLine("Hello VerifyBasicAdd"); });
                    break;

                case "VerifyAddBasicSubScenario":
                    this.Add(() => { Logger.WriteLine("Hello VerifyBasicAdd"); });
                    this.Add(() => { Logger.WriteLine("There VerifyAddBasicSubScenario"); });
                    break;
            }
        }
    }
}
