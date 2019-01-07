using ScenarioExecutionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionTests.Factories
{
    public class CustomBuilderFactory : QuickFactory
    {
        public override Dictionary<string, Func<IScenario>> ScenarioMap
        {
            get
            {
                return new Dictionary<string, Func<IScenario>>
                {
                    {"VerifyBasicAddForCustomBuilder" ,()=> 
                                    {
                                        return new CustomBuilder()
                                                    .AddTestStep()
                                                    .Build(); }
                                    }
                    };
            }

        }
    }
}
