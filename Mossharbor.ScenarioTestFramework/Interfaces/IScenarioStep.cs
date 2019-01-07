using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    public interface IScenarioStep
    {
        void SetUp();
        void ExecuteStep();
        void CleanUp();
        void EndRunCleanUp();
    }
}
