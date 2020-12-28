using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionTests
{
    using Mossharbor.ScenarioTestFramework;

    public class BasicQuickFactory : QuickFactory
    {
        public override Dictionary<string, Func<IScenario>> ScenarioMap
        {
            get
            {
                return new Dictionary<string, Func<IScenario>>
                {
                   {"VerifyBasicAdd"             ,()=> { return new Scenario(() => { Logger.WriteLine("Hello VerifyBasicAdd"); }); }}
                  ,{"VerifyAddBasicSubScenario"  ,()=> 
                  { 
                      var v = new Scenario(() => { Logger.WriteLine("Hello VerifyBasicAdd"); });
                      v.Add(() => { Logger.WriteLine("There VerifyAddBasicSubScenario");});
                      return v;
                  }}
                };
            }
        }
    }
}
