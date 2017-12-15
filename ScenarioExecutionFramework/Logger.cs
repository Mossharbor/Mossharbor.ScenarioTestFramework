using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioExecutionFramework
{
    using ScenarioExecutionFramework.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class Logger
    {
        IScenario currentScenario = null;
        /// <summary>
        /// Log providers to invoke for each logging call
        /// </summary>
        private List<ILogProvider> logProviders = null;

        /// <summary>
        /// Current subresult logged for test (only valid when in Start/End block)
        /// </summary>
        private Result curSubResult;
        /// <summary>
        /// The cumulative result for this test case.
        /// </summary>
        /// <remarks>
        /// This property is used to set or get the current sub-result.  If you 
        /// set the result, it will compare it to the existing result making sure
        /// that the correct final result is preserved.  Consider the following
        /// three lines of code:
        /// <code>
        /// // Sets the result from Result.NoResult to Result.Pass
        /// CurrentSubResult = Result.Pass;
        /// 
        /// // Sets the result to Result.Fail
        /// CurrentSubResult = Result.Fail;
        /// 
        /// // Result.Pass is less than Result.Fail, so this line 
        /// // has no effect.
        /// CurrentSubResult = Result.Pass;
        /// </code>
        /// After executing the above code, the value of this property will be
        /// <c>Result.Fail</c>.
        /// </remarks>
        /// <value>The most-recently computed result for the current test case</value>
        public Result CurrentSubResult
        {
            get
            {
                return this.curSubResult;
            }

            set
            {
                if (value > this.curSubResult)
                    this.curSubResult = value;
            }
        }

        /// <summary>
        /// The current time, updated before calling through the list of
        /// LogProviders
        /// </summary>
        private DateTime currentTime;
        /// <summary>
        /// Returns the current time for the benefit of any ILogProvider
        /// implementation.
        /// </summary>
        public DateTime CurrentTime
        {
            get
            {
                return this.currentTime;
            }

            internal set
            {
                this.currentTime = value;
            }
        }

        /// <summary>
        /// This is the time that the scenario has started
        /// </summary>
        private DateTime scenarioStartTime;
        public DateTime ScenarioStartTime
        {
            get
            {
                return this.scenarioStartTime;
            }

            private set
            {
                this.scenarioStartTime = value;
            }
        }

        /// <summary>
        /// Returns the time that has elapsed since the start of scenario execution.
        /// </summary>
        public TimeSpan ScenarioElapsedTime
        {
            get
            {
                return DateTime.Now - ScenarioStartTime;
            }
        }

        /// <summary>
        /// The directory where all log files are kept
        /// </summary>
        private string logFileDirectory;
        /// <summary>
        /// Returns the directory that all LogProviders should keep their files in.
        /// </summary>
        public string LogFileDirectory
        {
            get
            {
                return logFileDirectory;
            }

            set
            {
                if (loggerStarted)
                    throw new CannotChangeLogDirectoryAfterLoggerHasStartedException();

                logFileDirectory = value;
            }
        }

        /// <summary>
        /// The default log file name for any log providers that want it
        /// </summary>
        private string logFileName;
        /// <summary>
        /// Default log file name without extension
        /// </summary>
        /// <remarks>Remember to add the file extension to the file name</remarks>
        public string LogFileName
        {
            get
            {
                if (String.IsNullOrEmpty(this.logFileName))
                {
                    // Get running module name
                    string moduleName = ((this.currentScenario.Assembly.Length > 0) ? this.currentScenario.Assembly : "UnknownModule");

                    // Get default file name
                    this.logFileName = String.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.log", moduleName, DateTime.Now);

                    // Check for parameter override
                    this.logFileName = RuntimeParameters.Instance.GetCommandLineOptionDefault("thlog", this.logFileName);

                    // Add log path
                    this.logFileName = Path.Combine(Logger.Instance.LogFileDirectory, this.logFileName);
                }

                return this.logFileName;
            }
        }

        /// <summary>
        /// Used for creating link to debug Message Log
        /// </summary>
        private string debugMessageLogDir;
        /// <summary>
        /// Returns the directory that all LogProviders should keep their files in.
        /// </summary>
        public string DebugMessageLogDir
        {
            get
            {
                return debugMessageLogDir;
            }

            private set
            {
                debugMessageLogDir = value;
            }
        }

        /// <summary>
        /// Used for creating link to screenshot images
        /// </summary>
        private string screenshotDir;
        /// <summary>
        /// Used for creating link to screenshot images
        /// </summary>
        public string ScreenShotDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(this.screenshotDir) && !String.IsNullOrEmpty(LogFileDirectory) && Directory.Exists(this.LogFileDirectory))
                    this.screenshotDir = Path.Combine(LogFileDirectory, "ScreenShots");

                return this.screenshotDir;
            }
            set { this.screenshotDir = value; }
        }

        /// <summary>
        /// True if initialization is complete
        /// </summary>
        private bool isInitComplete;

        /// <summary>
        /// This is set to true once the StartLogger function has been called.
        /// </summary>
        private bool loggerStarted = false;

        /// <summary>
        /// True to log every result, otherwise just a general pass / fail
        /// </summary>
        /// <remarks>Currently only effects WTT log provider</remarks>
        private bool logAllResults = false;
        /// <summary>
        /// True to log every result, otherwise just a general pass / fail
        /// </summary>
        /// <remarks>Currently only effects some providers</remarks>
        public bool LogAllResults
        {
            get { return this.logAllResults; }
            set { this.logAllResults = value; }
        }

        /// <summary>
        /// This is set by the infrastructure for storing exceptions that have been thrown
        /// </summary>
        /// <remarks>This is most useful when there is a catastrophic error before a test has run.</remarks>
        public Exception LastExceptionThrown
        {
            get;
            internal set;
        }

        /// <summary>
        /// Enable logged results summary
        /// </summary>
        private LogResults logResultSummary = LogResults.AllResults;
        /// <summary>
        /// Enable logged results summary
        /// </summary>
        public LogResults LogResultSummary
        {
            get { return this.logResultSummary; }
            set { this.logResultSummary = value; }
        }

        /// <summary>
        /// Log provider activity list
        /// </summary>
        private Dictionary<string, bool> logProviderActivityList = new Dictionary<string, bool>();

        /// <summary>
        /// Logged sub results
        /// </summary>
        private Dictionary<string, List<LogResult>> loggedResults = null;
        /// <summary>
        /// Currently logged results, organized by scenario
        /// </summary>
        public Dictionary<string, List<LogResult>> LoggedResults
        {
            get
            {
                if (this.loggedResults == null)
                {
                    this.loggedResults = new Dictionary<string, List<LogResult>>();
                }

                return this.loggedResults;
            }
        }

        /// <summary>
        /// True if within StartScenario / StopScenario block
        /// </summary>
        public bool ScenarioIsRunning
        {
            get { return this.currentScenario != null; }
        }

        public Logger()
        {
            // Make sure the logfile directory exists
            LogFileDirectory = Path.Combine(System.Environment.CurrentDirectory, "Logs");
            this.curSubResult = Result.NoResult;
            this.scenarioStartTime = new DateTime(0);
            this.logProviders = new List<ILogProvider>();

            DefaultLogProvider lp = new DefaultLogProvider(Console.Out, "ConsoleLogProvider");
            this.logProviders.Add(lp);
            logProviderActivityList.Add(lp.LogProviderName, true);

            lp = new DefaultLogProvider(new DebugOutputTextWriter(), "DebugOutputLogProvider");
            this.logProviders.Add(lp);
            logProviderActivityList.Add(lp.LogProviderName, true);

            try
            {
                //TODO LoadLogProviders(); from config somehow
            }
            catch (Exception ex)
            {
                // This constructor can NOT throw an exception, since all
                // info about the exception gets lost.  Just write out an
                // error and hope for the best.
                WriteError("Exception initializing Logger\n{0}\n{1}", ex.Message, ex.StackTrace);
                WriteError("InnerException:" + ex.InnerException.Message, ex.InnerException.StackTrace);
                return;
            }
        }

        public void LoadLogProvider(string providerAssembly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function loads the log provider specified. The loaded log providers are enabled by default with this method.
        /// </summary>
        /// <param name="lp">This is the log provider that we would like to add to the list.</param>
        public void LoadLogProvider(ILogProvider lp)
        {
            this.LoadLogProvider(lp, true);
        }

        /// <summary>
        /// This function loads a log provider and set whether it's enabled or disabled.
        /// </summary>
        /// <param name="lp">this the log provider to add to the list of log providers.</param>
        /// <param name="enabled">this indicates if the log provider is enabled or disbled.</param>
        public void LoadLogProvider(ILogProvider lp, bool enabled)
        {
            if (!logProviderActivityList.ContainsKey(lp.LogProviderName))
            {
                logProviderActivityList.Add(lp.LogProviderName, enabled);
                this.logProviders.Add(lp);
                System.Diagnostics.Debug.WriteLine("ILogProvider {0} Loaded.", lp.LogProviderName.Replace("Microsoft.TestAutomation.TestManagement.LogProviders.", ""));
            }
        }

        private void KillLogProvider(ILogProvider lp, Exception ex)
        {
            // Deactive the offending ILogProvider
            if (logProviderActivityList.ContainsKey(lp.LogProviderName))
                logProviderActivityList[lp.LogProviderName] = false;

            // Tell the other LogProviders, so someone can hear
            // about the problem.
            WriteException(null, ex, "ILogProvider {0} threw an exception, and will be deactivated.", lp.GetType());
        }

        private bool IsLogProviderActive(ILogProvider lp)
        {
            if (logProviderActivityList.ContainsKey(lp.LogProviderName))
                return logProviderActivityList[lp.LogProviderName];

            return false;
        }

        /// <summary>
        /// Called to initialize logging, before any result or tracing functions are called
        /// </summary>
        public void StartLogger()
        {
            this.loggerStarted = true;

            if (!Directory.Exists(LogFileDirectory))
            {
                Directory.CreateDirectory(LogFileDirectory);
            }

            // Call StartLogger for each ILogProvider
            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.StartLogger();
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }
        }

        /// <summary>
        /// This function announces to the log providers that it is starting the scenario.execution.
        /// </summary>
        public void StartScenario(IScenario scenario)
        {
            CurrentTime = ScenarioStartTime = DateTime.Now;

            // make sure we're not in a test
            if (ScenarioIsRunning)
            {
                throw new CantStartScenarioException(string.Format("StartStartScenario cannot be called while scenario {0} is currently running, ", this.currentScenario.Name));
            }

            this.currentScenario = scenario;

            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.StartScenario(scenario);
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }
        }

        /// <summary>
        /// This function annouces to the loggers that it is done running tests and the scenario is finished.
        /// </summary>
        public void StopScenario(IScenario scenario)
        {
            CurrentTime = DateTime.Now;

            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.StopScenario();
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }

            // Unset the state data
            this.currentScenario = null;
            this.curSubResult = Result.NoResult;
        }

        /// <summary>
        /// Called to log an intermediate result for a test.  When a test is finished,
        /// the results of all calls to LogSubResult are combined and a final result
        /// is logged.  Must be called once between StartTest and EndTest,
        /// or a failing result will be logged.
        /// </summary>
        /// <param Name="result">Result to log</param>
        /// <param Name="format">Text associated with result</param>
        /// <param Name="arguments">Text arguments to put in format string</param>
        public void LogSubResult(Result result, string format, params object[] arguments)
        {
            LogSubResult(result, null, format, arguments);
        }

        /// <summary>
        /// Called to log an intermediate result for a test.  When a test is finished,
        /// the results of all calls to LogSubResult are combined and a final result
        /// is logged.  Must be called once between StartTest and EndTest,
        /// or a failing result will be logged.
        /// </summary>
        /// <param Name="result">Result to log</param>
        /// <param name="exception">An exception that occurred (can be null)</param>
        /// <param Name="format">Text associated with result</param>
        /// <param Name="arguments">Text arguments to put in format string</param>
        public void LogSubResult(Result result, Exception exception, string format, params object[] arguments)
        {
            // Make sure we're in a test
            if (!ScenarioIsRunning)
            {
                throw new CanLogResultWhenScenarioIsNotRunningException("No test is currently running, LogSubResult cannot be called");
            }

            // Update the current result. 
            CurrentSubResult = result;

            if ((result == Result.Fail || result == Result.Exception || result == Result.ProductException || result == Result.Error) && RuntimeParameters.Instance.BreakOnFail) //TODO break on fail
                System.Diagnostics.Debugger.Break();

            // Set the current time
            CurrentTime = DateTime.Now;

            // Format the string if we need to
            if (arguments != null && arguments.Length > 0)
            {
                format = string.Format(CultureInfo.InvariantCulture, format, arguments);
            }

            if (!String.IsNullOrEmpty(currentScenario.Name) && ((this.LogResultSummary == LogResults.AllResults) || ((this.LogResultSummary == LogResults.FailsOnly) && (result > Result.Pass))))
            {
                // Add sub result to list. We only do so when not running in stress mode. When in stress mode,
                // Test cases can run for thousands of iterations, and the last thing we want is to be adding the result to
                // an internal collection that will keep on growing upbounded and become bigger and bigger. We
                // might just create an internal memory leak condition within the TestManagement infrastructure itself.
                if (!this.LoggedResults.ContainsKey(currentScenario.Name))
                {
                    this.LoggedResults.Add(currentScenario.Name, new List<LogResult>());
                    this.loggedResults[currentScenario.Name].Add(new LogResult(currentScenario, result, format));
                }
                else if (result > Result.Pass && this.loggedResults[currentScenario.Name].Count < 10)
                    this.loggedResults[currentScenario.Name].Add(new LogResult(currentScenario, result, format));
            }

            // Call LogSubResult for each ILogProvider
            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.LogSubResult(result, exception, format);
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }
        }

        public void LogIf(bool condition, string passString, string failString)
        {
            if (condition)
                LogSubResult(Result.Pass, passString);
            else
                LogSubResult(Result.Fail, failString);
        }

        /// <summary>
        /// Create screenshot for every logProvider
        /// </summary>
        /// <param name="fullPathToImageFile">default is null.</param>
        public string LogScreenShot(string screenShotFileName)
        {
            screenShotFileName = this.GenerateScreenShot(screenShotFileName);

            // If screen shot created
            if (!String.IsNullOrEmpty(screenShotFileName))
            {
                // Call LogSubResult for each ILogProvider
                foreach (ILogProvider curProvider in this.logProviders)
                {
                    if (!IsLogProviderActive(curProvider))
                    {
                        continue;
                    }

                    try
                    {
                        curProvider.LogScreenShot(screenShotFileName);
                    }
                    catch (Exception ex)
                    {
                        this.KillLogProvider(curProvider, ex);
                    }
                }
            }

            return screenShotFileName;
        }

        /// <summary>
        /// Adds specified file to log copy list
        /// </summary>
        /// <param name="fileName">Full path to file to add to list</param>
        public void LogFileAdd(string fileName)
        {
            // we don't want to log screenshot
            if (!String.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                // Call LogSubResult for each ILogProvider
                foreach (ILogProvider curProvider in this.logProviders)
                {
                    if (!IsLogProviderActive(curProvider))
                    {
                        continue;
                    }

                    try
                    {
                        curProvider.LogFile(fileName);
                    }
                    catch (Exception ex)
                    {
                        this.KillLogProvider(curProvider, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Writes an array of lines of text to the output targets.
        /// </summary>
        /// <param Name="message">An array of lines of text to write</param>
        public void WriteLine(string[] message)
        {
            foreach (string line in message)
            {
                this.WriteLine(line);
            }
        }

        /// <summary>
        /// Writes a line of debug text of specified type
        /// </summary>
        /// <param name="messageType">Metadata tag for this message</param>
        /// <param name="message">Line of text to write</param>
        /// <remarks>
        /// The message type was added for the xml log provider to parse specific messages
        /// This will log debug messages as a type other than "Info" or "Error".
        /// </remarks>
        public void WriteLineWithType(string messageType, string format, params object[] arguments)
        {
            // Ignore the call if there's no format string
            if (format == null)
            {
                return;
            }

            // Set the current time
            CurrentTime = DateTime.Now;

            // Format the string if we need to
            if (arguments != null && arguments.Length > 0)
            {
                format = string.Format(CultureInfo.InvariantCulture, format, arguments);
            }

            // Call WriteLine for each ILogProvider
            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    if (String.IsNullOrEmpty(messageType))
                    {
                        cur.WriteLine(format);
                    }
                    else
                    {
                        cur.WriteLine(messageType, format);
                    }
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }
        }

        /// <summary>
        /// Writes a line of text to the output targets with specified highlighting type.
        /// </summary>
        /// <param Name="messageType">Message highlight type</param>
        /// <param Name="format">Text to write</param>
        /// <param Name="arguments">Text arguments to put in format string</param>
        public void WriteLine(MessageHighlightType messageType, string format, params object[] arguments)
        {
            this.WriteLineWithType("Highlight" + messageType.ToString(), format, arguments);
        }

        /// <summary>
        /// Writes a line of text to the output targets.
        /// </summary>
        /// <param Name="format">Text to write</param>
        /// <param Name="arguments">Text arguments to put in format string</param>
        public void WriteLine(string format, params object[] arguments)
        {
            this.WriteLineWithType(null, format, arguments);
        }

        /// <summary>
        /// Writes an array of lines of text to the output targets.  This text can be
        /// formatted/handled slightly differently by the output targets, due
        /// to its higher priority nature vs. normal debug output.
        /// </summary>
        /// <param Name="errorMessage">An array of lines of text to write</param>
        public void WriteError(string[] message)
        {
            foreach (string line in message)
            {
                WriteError(line);
            }
        }

        /// <summary>
        /// Writes a line of text to the output targets.  This text can be
        /// formatted/handled slightly differently by the output targets, due
        /// to its higher priority nature vs. normal debug output.
        /// </summary>
        /// <param Name="format">Text to write</param>
        /// <param Name="arguments">Text arguments to put in format string</param>
        public void WriteError(string format, params object[] arguments)
        {
            // Ignore the call if there's no format string
            if (format == null)
            {
                return;
            }

            WriteException(null, null, format, arguments);
        }

        /// <summary>
        /// Writes an exception.  This is a fancy overload for WriteError.
        /// </summary>
        /// <param name="exception">Your exception</param>
        /// <param name="format">A description of what was happening when you caught the exception.</param>
        /// <param name="arguments">Any other arguments you have.</param>
        public void WriteException(Result? result, Exception exception, string format, params object[] arguments)
        {
            System.Diagnostics.Debug.Assert(null == result || result == Result.Exception || result == Result.ProductException || result == Result.Timeout);

            // Set the current time
            int numberOfErrorsWritten = 0;
            CurrentTime = DateTime.Now;

            // Format the string if we need to
            if (arguments != null && arguments.Length > 0)
            {
                format = string.Format(CultureInfo.InvariantCulture, format, arguments);
            }

            // Call WriteError for each ILogProvider
            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.WriteException(exception, format);
                    ++numberOfErrorsWritten;
                }
                catch (Exception ex)
                {
                    if (logProviderActivityList.ContainsKey(cur.LogProviderName))
                        logProviderActivityList[cur.LogProviderName] = false;

                    Console.WriteLine("Error in WriteError for " + cur.LogProviderName + ":" + ex.Message + System.Environment.NewLine + ex.StackTrace + System.Environment.NewLine);

                    if (ex.InnerException != null)
                        Console.WriteLine("InnerException " + ex.InnerException.Message + System.Environment.NewLine + ex.InnerException.StackTrace + System.Environment.NewLine);
                }

                if (0 == numberOfErrorsWritten)
                {
                    Console.WriteLine(format, arguments);

                    if (null != exception)
                        Console.WriteLine(exception.Message + System.Environment.NewLine + exception.StackTrace + System.Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Logs a result for the given test ID.  If called while within Start/EndTest,
        /// this functions like LogSubResult.  Otherwise, if functions like StartTest,
        /// LogSubResult, EndTest.
        /// </summary>
        /// <param Name="id">ID to log for.  This parameter is ignored if StartTest has been called.</param>
        /// <param Name="result">Result to log</param>
        /// <param Name="format">Text associated with result</param>
        /// <param Name="arguments">Text arguments to put in format string</param>
        public void LogResult(IScenario id, Result result, string format, params object[] arguments)
        {
            string message = string.Format(CultureInfo.InvariantCulture, format, arguments);

            if (ScenarioIsRunning)
            {
                // this result is for the currently executing test, log it as a subresult ignoring Id
                this.LogSubResult(result, message);
                return;
            }

            //TODO: this should throw an exception!!
            this.StartScenario(id);
            this.LogSubResult(result, message);
            this.StopScenario(id);
        }

        /// <summary>
        /// Writes a URL out to the log files
        /// </summary>
        /// <param name="url">The URL you want to write</param>
        public void WriteUrlLink(string url)
        {
            WriteUrlLink(url, url);
        }

        /// <summary>
        /// Writes a URL out to the log files
        /// </summary>
        /// <param name="description">The hypertext description for the url</param>
        /// <param name="url">The actual url address</param>
        public void WriteUrlLink(string description, string url)
        {
            WriteUrlLink(description, url, false);
        }

        /// <summary>
        /// Writes a URL out to the log files
        /// </summary>
        /// <param name="description">The hypertext description for the url</param>
        /// <param name="url">The actual url address</param>
        /// <param name="copyFile">True to copy linked file (WTT log)</param>
        public void WriteUrlLink(string description, string url, bool copyFile)
        {
            // Set the current time
            CurrentTime = DateTime.Now;

            // Call WriteError for each ILogProvider
            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.WriteUrlLink(description, url, copyFile);
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }
        }

        /// <summary>
        /// Called to clean up logging, after all traces and results have been written
        /// </summary>
        public void StopLogger()
        {
            // Set the current time
            CurrentTime = DateTime.Now;

            if (ScenarioIsRunning)
            {
                string stopLoggerError = "Logger.StopLogger() was called before Logger.EndTest().";

                if (CurrentSubResult == Result.NoResult)
                {
                    LogSubResult(Result.Error, stopLoggerError + " Logging an error; please investigate");
                }
                else
                {
                    WriteError(stopLoggerError);
                }

                StopScenario(null);
            }

            // Call StopLogger for each ILogProvider
            foreach (ILogProvider cur in this.logProviders)
            {
                if (!IsLogProviderActive(cur))
                {
                    continue;
                }

                try
                {
                    cur.StopLogger();
                }
                catch (Exception ex)
                {
                    KillLogProvider(cur, ex);
                }
            }

            this.currentScenario= null;
        }

        /// <summary>
        /// Generate the actual screen shot
        /// </summary>
        /// <param name="screenShotFileName">File name of screen shot image</param>
        /// <returns>Screen shot file name, null if error</returns>
        private string GenerateScreenShot(string screenShotFileName)
        {
            try
            {
                // If we don't want to log a screenshot
                if (String.IsNullOrEmpty(screenShotFileName))
                {
                    return null;
                }

                // If ee is open (and why wouldn't it be?)
                this.MinimizeApp();

                // Check the screensaver
                bool screensaver = false;

                SystemParametersInfo(GetScreenSaverActive, 0, ref screensaver, 0);

                // If screensaver enabled
                if (screensaver)
                {
                    Logger.Instance.LogSubResult(Result.InfrastructureWarning, "Screensaver is currently enabled (should be disabled for testing)");
                    SystemParametersInfo(GetScreenSaverRunning, 0, ref screensaver, 0);

                    // If screensaver is running
                    if (screensaver)
                    {
                        // Make sure machine is awake (press ctrl key)
                        Logger.Instance.LogSubResult(Result.InfrastructureWarning, "Screensaver is currently running");
                    }
                }

                // Check if machine is locked
                int desktopHandle = 0;

                try
                {
                    desktopHandle = OpenInputDesktop(0, false, Desktop_SwitchDesktop);
                }
                finally
                {
                    // If desktop handle was opened
                    if (desktopHandle != 0)
                    {
                        // Close the opened desktop handle
                        CloseDesktop(desktopHandle);
                    }
                    else
                    {
                        Logger.Instance.LogSubResult(Result.InfrastructureWarning, "Could not access window input handle, machine is probably locked");
                    }
                }

                // Make sure the directory exists
                if (!Directory.Exists(Logger.Instance.ScreenShotDirectory))
                {
                    Directory.CreateDirectory(Logger.Instance.ScreenShotDirectory);
                }

                // Get the screen shot
                string screenShotFilenameAndPath = ScreenCapture.CaptureScreenshotWithFileName(Logger.Instance.ScreenShotDirectory, screenShotFileName);

                if (String.IsNullOrEmpty(screenShotFilenameAndPath))
                {
                    Logger.Instance.LogSubResult(Result.InfrastructureWarning, null, "Couldn't create screenshot!");
                    return null;
                }

                return screenShotFilenameAndPath;
            }
            catch (Exception e)
            {
                Logger.Instance.LogSubResult(Result.InfrastructureWarning, e, "Couldn't create screenshot!");
                return null;
            }
        }

        /// <summary>
        /// Minimize main window of the current process
        /// </summary>
        /// <returns>True if at least one app was sent a minimize message</returns>
        /// <remarks>
        /// If process has no main window, this function has no effect.
        /// </remarks>
        private bool MinimizeApp()
        {
            bool minimized = false;

            Process process = Process.GetCurrentProcess();

            // If found minimize it
            if (process != null)
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    // Send message to this window to minimize
                    SendMessage(process.MainWindowHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
                    minimized = true;

                    // Wait for it to minimize
                    System.Threading.Thread.Sleep(200);
                }
            }

            return minimized;
        }
        #region External Dlls

        /// <summary>
        /// System window command
        /// </summary>
        private const int WM_SYSCOMMAND = 0x0112;

        /// <summary>
        /// Minimize window
        /// </summary>
        private const int SC_MINIMIZE = 0xF020;

        /// <summary>
        /// Maximize window
        /// </summary>
        private const int SC_MAXIMIZE = 0xF030;

        /// <summary>
        /// Close window
        /// </summary>
        private const int SC_CLOSE = 0xF060;

        /// <summary>
        /// Action to query if screensaver is active
        /// </summary>
        private const int GetScreenSaverActive = 16;

        /// <summary>
        /// Action to set if screensaver is active
        /// </summary>
        private const int SetScreenSaverActive = 17;

        /// <summary>
        /// Action to query if screensaver is currently running
        /// </summary>
        private const int GetScreenSaverRunning = 114;

        /// <summary>
        /// Security permissions to switch desktop
        /// </summary>
        private const uint Desktop_SwitchDesktop = 0x0100;

        /// <summary>
        /// Close opened desktop handle
        /// </summary>
        /// <param name="desktop">Desktop handle to close</param>
        /// <returns>True if handle was closed</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool CloseDesktop(int desktop);

        /// <summary>
        /// Open current input desktop window handle
        /// </summary>
        /// <param name="flags">Open desktop flags</param>
        /// <param name="inherit">True to inherit from parent window</param>
        /// <param name="desiredAccess">Security permissions to open with</param>
        /// <returns>Opened desktop handle (0 if could not open handle)</returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int OpenInputDesktop(uint flags, bool inherit, uint desiredAccess);

        /// <summary>
        /// Windows send message
        /// </summary>
        /// <param name="hWnd">Message handle to send message to</param>
        /// <param name="Msg">Message to send</param>
        /// <param name="wParam">WPARAM parameter</param>
        /// <param name="lParam">LPARAM parameter</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Get or Set system parameter informaion (such as screensaver)
        /// </summary>
        /// <param name="action">System action item</param>
        /// <param name="param">Input parameter</param>
        /// <param name="outParam">Output parameter</param>
        /// <param name="ignore">Ignored value</param>
        /// <returns>True if operation successful</returns>
        [DllImport("user32.dll")]
        private static extern bool SystemParametersInfo(int action, int param, ref bool outParam, int ignore);

        #endregion
        /// <summary>
        /// This property is used to return the singleton instance of the logger.
        /// The Logger object is automatically created and initialized the first 
        /// time this property is read.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                lock (SingletonLoggerInstance.Instance)
                {
                    return SingletonLoggerInstance.Instance;
                }
            }
        }

        /// <summary>
        /// This is a class that handles the singleton Logger instance.
        /// It is not exposed to anythigng because we only want one instance of
        /// Logger at a time.
        /// </summary>
        private class SingletonLoggerInstance
        {
            /// <summary>
            /// Don't let ANYONE use new to make one of these
            /// </summary>
            private SingletonLoggerInstance()
            {
            }


            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            /// </summary>
            static SingletonLoggerInstance()
            {
            }

            /// <summary>
            /// Static read-only field used to keep the reference to the 
            /// singleton object.
            /// </summary>
            internal static readonly Logger Instance = new Logger();
        }
    }
}
