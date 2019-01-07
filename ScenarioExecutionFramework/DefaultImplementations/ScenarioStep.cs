using ScenarioExecutionFramework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScenarioExecutionFramework
{
    internal class ScenarioStep :IScenarioStep
    {
        Action toSetup = null;
        Action toCleanup = null;
        Action toExecute = null;
        Action toEndRunCleanup = null;

        public ScenarioStep(Action toExecute)
        {
            this.toExecute = toExecute;
        }

        public void CleanUp()
        {
            if (null != toCleanup)
                toCleanup();
        }

        public void EndRunCleanUp()
        {
            if (null != toEndRunCleanup)
                toEndRunCleanup();
        }

        public void ExecuteStep()
        {
            if (null == toExecute)
                throw new ExecutionStepIsEmptyException();

            toExecute();
        }

        public void SetUp()
        {
            if (null != toSetup)
                toSetup();
        }
    }
}
