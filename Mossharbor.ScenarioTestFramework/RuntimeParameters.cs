using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Mossharbor.ScenarioTestFramework
{
    public class RuntimeParameters
    {
        /// <summary>
        /// This is an action that you want to the test case to take upon connection with an app debugger.
        /// </summary>
        private DebugAction dbgrAction;
        /// <summary>
        /// Number of times to run the scenario
        /// </summary>
        private int scenarioLoops;

        /// <summary>
        /// Number of times to run each test
        /// </summary>
        private int executionLoops;

        /// <summary>
        /// If we are using the CheckTestStability command line flag this is the number that was passed in.
        /// </summary>
        private int scenarioStabilityCount;

        /// <summary>
        /// The test build number
        /// </summary>
        private string buildNumber;

        /// <summary>
        /// Path to a file to get the file version build number from
        /// </summary>
        private string buildNumberPath;

        /// <summary>
        /// indicates if we need to break into the debugger when we fail
        /// </summary>
        private bool breakOnFail = false;

        /// <summary>
        /// Parsed arguments class
        /// </summary>
        private ArgumentParser pargs;

        /// <summary>
        /// constructor
        /// </summary>
        private RuntimeParameters()
        {
            this.DebugAction = DebugAction.None;
            this.scenarioLoops = 1;
            this.executionLoops = 1;
            this.scenarioStabilityCount = 0;
        }

        #region *** Debug Parameters *************************************************
        /// <summary>
        /// This is a Debug Action that you would like the test case to take if 
        /// An application debugger is attached to the test.
        /// </summary>
        public DebugAction DebugAction
        {
            get { return this.dbgrAction; }
            set { this.dbgrAction = value; }
        }

        /// <summary>
        /// Build number the test is running against from the command line.
        /// </summary>
        public string BuildNumber
        {
            get
            {
                // If build number is already set
                if (!String.IsNullOrEmpty(this.buildNumber))
                {
                    return this.buildNumber;
                }

                // If build version file path specified
                if (!String.IsNullOrEmpty(this.BuildNumberSourceModule))
                {
                    if (File.Exists(this.BuildNumberSourceModule))
                    {
                        // Get the file version info of specified file
                        FileVersionInfo version = FileVersionInfo.GetVersionInfo(this.BuildNumberSourceModule);

                        if (version != null)
                        {
                            this.buildNumber = version.FileBuildPart.ToString();

                            if (version.FilePrivatePart != 0)
                            {
                                this.buildNumber += "." + version.FilePrivatePart.ToString();
                            }
                        }
                        else
                        {
                            InternalLogger.Instance.LogSubResult(Result.InfrastructureWarning, "Could not attain build number from specified source module '{0}'", this.BuildNumberSourceModule);
                        }
                    }
                    else
                    {
                        InternalLogger.Instance.LogSubResult(Result.InfrastructureWarning, "Could not find specified build number source module '{0}'", this.BuildNumberSourceModule);
                    }
                }

                // If the build number is still not set and it is specified in the command line
                if (String.IsNullOrEmpty(this.buildNumber) && this.GetCommandLineOptionExists(CommandLineSwitches.BuildNumber))
                {
                    this.buildNumber = this.GetCommandLineOptionValue(CommandLineSwitches.BuildNumber);

                    // Make sure the string is valid
                    if (String.IsNullOrEmpty(this.buildNumber))
                    {
                        InternalLogger.Instance.LogSubResult(Result.InfrastructureWarning, "Build number parameter specified in the command line, but is null or empty");
                    }
                }

                // If the build version is still not set, read it from the process version
                if (String.IsNullOrEmpty(this.buildNumber))
                {
                    Process process = Process.GetCurrentProcess();

                    if ((process.MainModule != null) && (process.MainModule.FileVersionInfo != null))
                    {
                        // Use the process file version
                        this.buildNumber = process.MainModule.FileVersionInfo.FileBuildPart.ToString();

                        if (process.MainModule.FileVersionInfo.FilePrivatePart != 0)
                        {
                            this.buildNumber += "." + process.MainModule.FileVersionInfo.FilePrivatePart.ToString();
                        }
                    }

                    if (String.IsNullOrEmpty(this.buildNumber))
                    {
                        InternalLogger.Instance.LogSubResult(Result.InfrastructureWarning, "Could not attain build number from the current process '{0}'", process.ProcessName);
                    }
                }

                // Build number may still be null, but at least we tried
                return this.buildNumber;
            }
        }

        /// <summary>
        /// Path to a file to get the file version build number from
        /// </summary>
        public string BuildNumberSourceModule
        {
            [DebuggerStepThrough]
            get { return this.buildNumberPath; }
        }

        /// <summary>
        /// indicates if we need to break into the debugger when we fail
        /// </summary>
        public bool BreakOnFail { get { return breakOnFail; } set { breakOnFail = value; } }

        public IEnumerable<string> CommandLineParameters
        {
            get
            {
                List<string> cmdLine = new List<string>();
                if (null == pargs)
                    return cmdLine;

                foreach (var t in pargs.Keys)
                {
                    cmdLine.Add(string.Format("{0}:{1}", (string)t, pargs[(string)t]));
                }

                return cmdLine;
            }
        }

        #endregion
        #region *** CommandLine Parameters *************************************************

        /// <summary>
        /// Initializes or re-initializes the RunTimeParameters object.  You can call this function
        /// as often as you like, but in mind that any previous command line settings will be
        /// discarded
        /// </summary>
        /// <param name="args">The command line arguments you want to set</param>
        public void Parse(string[] args)
        {
             pargs = new ArgumentParser(args);
        }

        public string AssemblyContainingFactory
        {
            get
            {
                if (!String.IsNullOrEmpty(assemblyContainingFactory))
                    return assemblyContainingFactory;

                string scenarioID = GetCommandLineOptionValue(CommandLineSwitches.ScenarioId);
                if (!scenarioID.Contains(":"))
                    return scenarioID;

                string[] splits = scenarioID.Split(":".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                assemblyContainingFactory = splits[0];

                return assemblyContainingFactory;
            }
        }
        private string assemblyContainingFactory = null;

        public string FactoryContainingScenario
        {
            get
            {
                if (!String.IsNullOrEmpty(factoryContainingScenario))
                    return factoryContainingScenario;

                string scenarioID = GetCommandLineOptionValue(CommandLineSwitches.ScenarioId);
                if (!scenarioID.Contains(":"))
                    return null;

                string[] splits = scenarioID.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (splits.Length <= 2)
                    return null;

                factoryContainingScenario = splits[1];

                return factoryContainingScenario;
            }
        }
        private string factoryContainingScenario = null;

        public string ScenarioNameToRun
        {
            get
            {
                if (!String.IsNullOrEmpty(scenarioNameToRun))
                    return scenarioNameToRun;

                string scenarioID = GetCommandLineOptionValue(CommandLineSwitches.ScenarioId);

                string[] splits = scenarioID.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (splits.Length == 1)
                    return null;

                if (splits.Length == 2)
                {
                    scenarioNameToRun = splits[1];
                    return scenarioNameToRun;
                }

                scenarioNameToRun = splits[3];

                return scenarioNameToRun;
            }
        }
        private string scenarioNameToRun = null;

        /// <summary>
        /// Gets an array of values from a command-line option
        /// </summary>
        /// <param name="optionName">Name of the switch to look up</param>
        /// <returns>A string array if the switch exists, or null.</returns>
        public string GetCommandLineOptionValue(string optionName)
        {
            return pargs[optionName];
        }

        /// <summary>
        /// Tests to see whether or not a command line option existst
        /// </summary>
        /// <param name="optionName">The name of the option you want to test for.</param>
        /// <returns><c>true</c> if the option exists, or <c>false</c> if it does not.</returns>
        public bool GetCommandLineOptionExists(string optionName)
        {
            if (null == pargs)
                return false;

            return pargs.ContainsParam(optionName);
        }


        /// <summary>
        /// Returns the value of a command line option, or a default value if the
        /// option does not exist.
        /// </summary>
        /// <param name="optionName">The name of the option you want to look up.</param>
        /// <param name="defaultValue">The default value for the option.</param>
        /// <returns>
        /// If the does not exist, the default value will be returned.  If the option 
        /// exists, but does not have a value, then the default value will be returned.
        /// Otherwise, the value given for the option will be returned.
        /// </returns>
        public string GetCommandLineOptionDefault(string optionName, string defaultValue)
        {
            if (null == this.pargs)
                return defaultValue;

            if (!this.pargs.ContainsParam(optionName))
            {
                return defaultValue;
            }

            string ret = this.pargs[optionName];
            if (ret == null)
            {
                return defaultValue;
            }

            return ret;
        }
            
        #endregion
        #region *** Scenario Run Parameters *************************************************

        /// <summary>
        /// This corresponds to the ScenarioestStability command line flag and indicates if it was set or not.
        /// </summary>
        /// <remarks>If the command line flag was specified then this will set ExecutionLoops the value passed in.
        /// Because of when this flag gets checked in the execution order it will override RepeatScenario command line parameters.</remarks>
        public bool CheckScenarioStability
        {
            get
            {
                if (this.scenarioStabilityCount < 1)
                {
                    bool commandLineOptionExists = this.GetCommandLineOptionExists(CommandLineSwitches.CheckTestStability);

                    if (!commandLineOptionExists)
                        return false;

                    // Try to parse command line for full stress loop number
                    if (!Int32.TryParse(this.GetCommandLineOptionValue(CommandLineSwitches.CheckTestStability), out this.scenarioStabilityCount))
                    {
                        //Command Line was specified but we failed to parse command argument default to 100
                        this.scenarioStabilityCount = 100;
                    }

                    //this.scenarioLoops = this.scenarioStabilityCount / 2;
                    this.executionLoops = this.scenarioStabilityCount;
                }

                return true;
            }
            set
            {
                if (value)
                    this.executionLoops = 100;
                else
                    this.executionLoops = 1;
            }
        }

        /// <summary>
        /// Number of times to run the scenario
        /// </summary>
        public int ScenarioLoops
        {
            get
            {
                // If uninitialized
                if (this.scenarioLoops < 1)
                {
                    this.scenarioLoops = 1;

                    // Try to parse command line for full stress loop number
                    if (this.GetCommandLineOptionExists(CommandLineSwitches.RepeatScenario)
                        && !Int32.TryParse(this.GetCommandLineOptionValue(CommandLineSwitches.RepeatScenario), out this.scenarioLoops))
                    {
                        // Parameter not specified or failed to parse command argument
                        this.scenarioLoops = 1;
                    }

                    // Must be greater than 0
                    if (this.scenarioLoops <= 0)
                    {
                        this.scenarioLoops = 1;
                    }
                }

                return this.scenarioLoops;
            }

            internal set { this.scenarioLoops = value; }
        }

        /// <summary>
        /// Number of times to run each test
        /// </summary>
        public int ExecutionLoops
        {
            get
            {
                // If uninitialized
                if (this.executionLoops < 1)
                {
                    this.executionLoops = 1;

                    // Try to parse command line for stress loop number
                    if (this.GetCommandLineOptionExists(CommandLineSwitches.RepeatTest)
                        && !Int32.TryParse(this.GetCommandLineOptionValue(CommandLineSwitches.RepeatTest), out this.executionLoops))
                    {
                        // Parameter not specified or failed to parse command argument
                        this.executionLoops = 1;
                    }

                    // Must be greater than 0
                    if (this.executionLoops <= 0)
                    {
                        this.executionLoops = 1;
                    }
                }

                return this.executionLoops;
            }

            set { this.executionLoops = value; }
        }

        /// <summary>
        /// This function checks the command line to see if we need to override the do scenario timeout (in seconds).
        /// </summary>
        /// <remarks>This return -1 if the command line option does not exist.</remarks>
        public int OverrideExecuteTimeout
        {
            get
            {
                if (!this.GetCommandLineOptionExists(CommandLineSwitches.ScenarioTimeout))
                    return -1;

                string str = this.GetCommandLineOptionValue(CommandLineSwitches.ScenarioTimeout);

                int testTimeoutInSeconds = 0;

                if (!Int32.TryParse(str, out testTimeoutInSeconds))
                    return -1;

                return testTimeoutInSeconds;
            }
        }

        /// <summary>
        /// This function checks the command line to see if we need to override the do scenario cleanup timeout (in seconds).
        /// </summary>
        /// <remarks>This return -1 if the command line option does not exist.</remarks>
        public int OverrideCleanupTimeout
        {
            get
            {
                if (!this.GetCommandLineOptionExists(CommandLineSwitches.ScenarioCleanupTimeout))
                    return -1;

                string str = this.GetCommandLineOptionValue(CommandLineSwitches.ScenarioCleanupTimeout);

                int testTimeoutInSeconds = 0;

                if (!Int32.TryParse(str, out testTimeoutInSeconds))
                    return -1;

                return testTimeoutInSeconds;
            }
        }

        /// <summary>
        /// This function checks the command line to see if we need to override the do scenario setup timeout (in seconds).
        /// </summary>
        /// <remarks>This return -1 if the command line option does not exist.</remarks>
        public int OverrideSetupTimeout
        {
            get
            {
                if (!this.GetCommandLineOptionExists(CommandLineSwitches.ScenarioSetupTimeout))
                    return -1;

                string str = this.GetCommandLineOptionValue(CommandLineSwitches.ScenarioSetupTimeout);

                int testTimeoutInSeconds = 0;

                if (!Int32.TryParse(str, out testTimeoutInSeconds))
                    return -1;

                return testTimeoutInSeconds;
            }
        }

        /// <summary>
        /// Indicates that we do not want to run the Scenario Setup functions
        /// </summary>
        public bool NoSetup
        {
            get { return bool.Parse(this.GetCommandLineOptionDefault(CommandLineSwitches.NoSetup,"false"));}
        }

        /// <summary>
        /// Indicates that we do not want to run the Scenario Cleanup functions
        /// </summary>
        public bool NoCleanup
        {
            get { return bool.Parse(this.GetCommandLineOptionDefault(CommandLineSwitches.NoCleanup, "false")); }
        }
    

        /// <summary>
        /// This is the priority to run the testing thread at once the test starts.
        /// </summary>
        public System.Threading.ThreadPriority ThreadPriority
        {
            get
            {
                string str = this.GetCommandLineOptionValue(CommandLineSwitches.ThreadPri);

                if (String.IsNullOrEmpty(str))
                    return System.Threading.ThreadPriority.Normal;

                int threadPriorityInt = (int)System.Threading.ThreadPriority.Normal;

                if (!Int32.TryParse(str, out threadPriorityInt))
                    return System.Threading.ThreadPriority.Normal;


                if (threadPriorityInt < (int)System.Threading.ThreadPriority.Lowest)
                    return System.Threading.ThreadPriority.Normal;

                if (threadPriorityInt > (int)System.Threading.ThreadPriority.Highest)
                    return System.Threading.ThreadPriority.Normal;

                return (System.Threading.ThreadPriority)threadPriorityInt;
            }
        }

        /// <summary>
        /// Disable all test cases Initialize Function calls if set
        /// </summary>
        public bool DisableInitializeFunctionCall
        {
            get { return this.GetCommandLineOptionExists(CommandLineSwitches.NoSetup); }
        }

        /// <summary>
        /// Disable all test cases Cleanup Function calls if set
        /// </summary>
        public bool DisableCleanupFunctionCall
        {
            get { return this.GetCommandLineOptionExists(CommandLineSwitches.NoSetup); }
        }

        /// <summary>
        /// No kill option.
        /// </summary>
        public bool NoKill
        {
            get { return this.GetCommandLineOptionExists(CommandLineSwitches.NoKill); }
        }


        #endregion
        #region *** Singleton ************************************************************
        #region *** Properties ***********************************************************

        /// <summary>
        /// This property is used to return the singleton instance of the logger.
        /// The TestParameters object is automatically created and initialized the first 
        /// time this property is read.
        /// </summary>
        public static RuntimeParameters Instance
        {
            get
            {
                lock (SingletonRuntimeParametersInstance.Instance)
                {
                    return SingletonRuntimeParametersInstance.Instance;
                }
            }
        }
        #endregion
        /// <summary>
        /// This is a class that handles the singleton TestParameters instance.
        /// It is not exposed to anythigng because we only want one instance of
        /// TestParameters at a time.
        /// </summary>
        private class SingletonRuntimeParametersInstance
        {
            /// <summary>
            /// Static read-only field used to keep the reference to the 
            /// singleton object.
            /// </summary>
            internal static readonly RuntimeParameters Instance = new RuntimeParameters();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            /// </summary>
            static SingletonRuntimeParametersInstance()
            {
            }

            /// <summary>
            /// Don't let ANYONE use new to make one of these
            /// </summary>
            private SingletonRuntimeParametersInstance()
            {
            }
        }
        #endregion
    }
}
