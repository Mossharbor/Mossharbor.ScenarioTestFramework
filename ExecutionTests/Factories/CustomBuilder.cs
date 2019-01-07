using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionTests
{
    using Mossharbor.ScenarioTestFramework;

    public class CustomBuilder : ScenarioBuilder
    {
        public CustomBuilder()
        {
            base.Begin();
        }

        public CustomBuilder AddTestStep()
        {
            base.AddStep(() => { Logger.Instance.LogSubResult(Result.Pass, "We Were Called"); });
            return this;
        }

        public override IScenario Build()
        {
            return base.Scenario;
        }
    }
}
