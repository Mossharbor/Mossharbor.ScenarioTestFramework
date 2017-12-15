using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace ScenarioExecutionFramework
{
    class CommandLineSwitches
    {
        /// <summary>
        /// Build number
        /// </summary>
        [Description("Specify the build number.")]
        public const string BuildNumber = "bld";

        /// <summary>
        /// Product Version
        /// </summary>
        [Description("Specify the product version.")]
        public const string ProductVersion = "prodvers";

        /// <summary>
        /// Set the thread priority for execution thread, 0=Lowest, 4=Highest");
        /// </summary>
        [Description("Set the thread priority for execution thread, 0=Lowest, 4=Highest")]
        public const string ThreadPri = "ThreadPriority";

        /// <summary>
        /// -id ScenarioDll:TestID:TacticsID indicates which test to run.
        /// </summary>
        [Description("The scenario to run. [-id ScenarioString]  [-id ScenarioDllName:ScenarioString] [-id ScenarioDllName:ScenarioFactory:ScenarioString] indicates which test to run.")]
        public const string ScenarioId = "id";

        /// <summary>
        /// Defines if the No Kill option was specified.
        /// </summary>
        [Description("Sets the NoKill flag to true. Scenarios interpret this per their needs (usually in cleanup)")]
        public const string NoKill = "nokill";

        /// <summary>
        /// This override the Scenario.Execute set inside of scenarios
        /// </summary>
        [Description("Set to override the Scenario.Execute timeout (in seconds) for running scenarios.")]
        public const string ScenarioTimeout = "ScenarioTimeout";

        /// <summary>
        /// This override the Scenario.Cleanup set inside of scenarios
        /// </summary>
        [Description("Set to override the Scenario.Cleanup timeout (in seconds) for running scenarios.")]
        public const string ScenarioCleanupTimeout = "ScenarioCleanupTimeout";

        /// <summary>
        /// This override the Scenario.Setup set inside of scenarios
        /// </summary>
        [Description("Set to override the Scenario.Setup timeout (in seconds) for running scenarios.")]
        public const string ScenarioSetupTimeout = "ScenarioSetupTimeout";

        /// <summary>
        /// Disable all the scenario setup calls.
        /// </summary>
        [Description("Set to disable the setup funtion call in a scenario.")]
        public const string NoSetup = "NoSetup";

        /// <summary>
        /// Disable all test case cleanup calls.
        /// </summary>
        [Description("Set to disable the Cleanup funtion call in a scenario.")]
        public const string NoCleanup = "NoCleanup";

        /// <summary>
        /// Run the entire scenario the specified number of times
        /// </summary>
        [Description("Runs the entire scenario the specified number of times. -repeatscenario #")]
        public const string RepeatScenario = "RepeatScenario";

        /// <summary>
        /// Run each execute call in the scenario the specified number of times.
        /// </summary>
        [Description("Runs each test the entire scenario the specified number of times. -repeatexec #")]
        public const string RepeatTest = "RepeatExec";

        /// <summary>
        /// Allows users to rerun their scenarios a large number of times running the whole scenario 50 times and running just the execution call 50 times.
        /// </summary>
        /// <seealso cref="RepeatExec"/>
        /// <seealso cref="RepeatScenario"/>
        [Description("Runs the Scenario X/2 and the Exeecute call X/2 times times to ensure consistent results. [-CheckTestStability X]")]
        public const string CheckTestStability = "CheckTestStability";

        /// <summary>
        /// Debug parameter.
        /// </summary>
        [Description("Allows for attaching the debugger to the scenario execution assisant.")]
        public const string DebugParam = "-dbg";

        /// <summary>
        /// Abort running instance of sea
        /// </summary>
        [Description("Allows for aborting all test cases. Will terminate any scenarios that are presently running.")]
        public const string AbortParam = "-abortall";

        /// <summary>
        /// List parameters.
        /// </summary>
        [Description("Lists all test cases contained in a factory. [-id factoryAssemblyName -list]")]
        public const string ListParam = "-list";

        /// <summary>
        /// Lists the possible command line arguments that can be passed
        /// </summary>
        [Description("Prints out the help information.")]
        public const string HelpOption1 = "-help";

        /// <summary>
        /// Lists the possible command line arguments that can be passed
        /// </summary>
        [Description("Prints out the help information.")]
        public const string HelpOption2 = "-?";

        /// <summary>
        /// Lists the possible command line arguments that can be passed.
        /// </summary>
        [Description("Prints out the help information.")]
        public const string HelpOption3 = "/?";
    }
}
