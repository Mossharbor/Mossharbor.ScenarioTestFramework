using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    public class Scenario : IScenario
    {
        public int ScenarioLoops { get { return toExec.Count; } }
        public int ExecutionLoops { get { return 1; } }
        private int executeTimeout = 0;
        public int ExecuteTimeout { get { if (0 == executeTimeout) executeTimeout = (int)TimeSpan.FromMinutes(15).TotalSeconds; return executeTimeout; } set { executeTimeout = value; } }
        public int CleanupTimeout { get { return (int)TimeSpan.FromMinutes(15).TotalSeconds; } }
        public int SetupTimeout { get { return (int)TimeSpan.FromMinutes(15).TotalSeconds; } }
        public string Name { get; set; }
        public string Assembly { get; set; }
        public string Factory { get; set; }
        public string Description { get; set; }
        public bool DisableTakingScreenShotOnFail { get; set; }

        int execIndex = -1;
        int lastExeIndex = -1;
        List<ScenarioItem> toExec = new List<ScenarioItem>();

        /// <summary>
        /// Indicates that exceptions that are thrown during 
        /// excecution are to be treated as product failure and not infrastruture failures
        /// </summary>
        public bool ExceptionAreProductExceptions
        {
            get { return exceptionAreProductExceptions; }
            set { exceptionAreProductExceptions = value; }
        }
        private bool exceptionAreProductExceptions = true;

        internal Scenario()
        {
        }

        public Scenario(string name, string module, string description, Action toSetup, Action toExecute, Action toCleanUp, Action endRunCleanup)
        {
            this.Name = name;
            this.Assembly = module;
            this.Description = description;
            toExec.Add(new ScenarioItem(toSetup, toExecute, toCleanUp, endRunCleanup));
        }
        
        public Scenario(string name, string module, string description, Action toExecute)
            :this(name, module, description, null, toExecute, null, null)
        {
        }

        public Scenario(Action toSetup, Action toExecute, Action toCleanUp, Action endRunCleanup)
        {
            toExec.Add(new ScenarioItem(toSetup, toExecute, toCleanUp, endRunCleanup));
        }

        public Scenario(Action toExecute)
        {
            toExec.Add(new ScenarioItem(null, toExecute, null, null));
        }

        internal Scenario(string name)
        {
            this.Name = name;
        }

        public void Add(Action toExecute)
        {
            toExec.Add(new ScenarioItem(null, toExecute, null, null));
        }

        public void Add(Action toInitialize, Action toExecute)
        {
            toExec.Add(new ScenarioItem(toInitialize, toExecute, null, null));
        }

        public void Add(Action toInitialize, Action toExecute, Action toCleanup)
        {
            toExec.Add(new ScenarioItem(toInitialize, toExecute, toCleanup, null));
        }

        public void Add(Action toInitialize, Action toExecute, Action toCleanup, Action toEndRunCleanup)
        {
            toExec.Add(new ScenarioItem(toInitialize, toExecute, toCleanup, toEndRunCleanup));
        }

        public void Add(IScenario subScenario)
        {
            toExec.Add(new ScenarioItem(()=>subScenario.CleanUp(), ()=>subScenario.Execute(), ()=>subScenario.CleanUp(), ()=>subScenario.EndRunCleanup()));
        }
        
        public void Add(IScenarioStep scenarioStep)
        {
            toExec.Add(new ScenarioItem(() => scenarioStep.CleanUp(), () => scenarioStep.ExecuteStep(), () => scenarioStep.CleanUp(), () => scenarioStep.EndRunCleanUp()));
        }

        public void SetUp()
        {
            if (lastExeIndex == execIndex)
                ++execIndex;
            ScenarioItem a = toExec.ElementAt(execIndex);
            if (null != a && null != a.Item1)
                a.Item1();
            lastExeIndex= execIndex;
        }

        public void Execute()
        {
            ScenarioItem a = toExec.ElementAt(execIndex);
            if (null != a && null != a.Item2)
                a.Item2();
        }

        public void CleanUp()
        {
            ScenarioItem a = toExec.ElementAt(execIndex);
            if (null != a && null != a.Item3)
                a.Item3();

            //if (execIndex >= toExec.Count)
            //{
            //    execIndex = -1;
            //    lastExeIndex = -1;
            //}
        }

        public void EndRunCleanup()
        {
            lastExeIndex = -1;
            execIndex = -1;
            for (int i = 0; i < toExec.Count; ++i)
            {
                ScenarioItem a = toExec.ElementAt(i);
                if (null != a && null != a.Item4)
                    a.Item4();
            }
            toExec.Clear();
        }
    }

    class ScenarioItem : Tuple<Action, Action, Action, Action>
    {
        public ScenarioItem(Action toInitialize, Action toExecute, Action toCleanUp, Action endRunCleanup)
            : base(toInitialize,toExecute,toCleanUp,endRunCleanup)
        {
        }
    }
}
