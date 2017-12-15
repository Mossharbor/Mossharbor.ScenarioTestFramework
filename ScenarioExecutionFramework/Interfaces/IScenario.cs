using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    public interface IScenario
    {
        int ScenarioLoops { get; }
        int ExecutionLoops { get; }
        int ExecuteTimeout { get; }
        int CleanupTimeout { get; }
        int SetupTimeout { get; }
        string Name { get; set; }
        string Assembly { get; set; }
        string Factory { get; set; }
        string Description { get; set; }
        /// <summary>
        /// This property overrides the default of taking a screen shot on failure.
        /// </summary>
        /// <remarks>Set to false to stop taking screen shots on failures.</remarks>
        bool DisableTakingScreenShotOnFail { get; set; }

        /// <summary>
        /// Indicates that exceptions that are thrown during 
        /// excecution are to be treated as product failure and not infrastruture failures
        /// </summary>
        bool ExceptionAreProductExceptions { get; }

        void SetUp();

        void Execute();

        void CleanUp();

        void EndRunCleanup();
    }
}
