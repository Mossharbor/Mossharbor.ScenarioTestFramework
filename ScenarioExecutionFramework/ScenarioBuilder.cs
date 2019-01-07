using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mossharbor.ScenarioTestFramework
{
    public abstract class ScenarioBuilder
    {
        private IScenario scenario = null;

        protected IScenario Scenario { get => scenario; set => scenario = value; }

        public ScenarioBuilder Begin(IScenario rootScenario)
        {
            Scenario = rootScenario;
            return this;
        }

        public ScenarioBuilder Begin()
        {
            return this.Begin(new Scenario());
        }

        public ScenarioBuilder AddStep(IScenarioStep step)
        {
            Scenario.Add(step);
            return this;
        }

        public ScenarioBuilder AddStep(Action action)
        {
            Scenario.Add(new ScenarioStep(action));
            return this;
        }

        public abstract IScenario Build();
    }
}
