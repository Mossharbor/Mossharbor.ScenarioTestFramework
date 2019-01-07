using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    using System;
    using System.IO;
    using System.Globalization;
    using System.Collections.Generic;

    /// <summary>
    /// Log provider that logs to a human-readable text stream
    /// </summary>
    public class DefaultLogProvider : ILogProvider, IDisposable
    {
        /// <summary>
        /// output stream
        /// </summary>
        private TextWriter stream;

        private string name = "TextStreamLogProvider";

        protected TextWriter Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        /// <summary>
        /// Returns a formatted string for the given argument.  If arg is null, returns empty string
        /// </summary>
        /// <param Name="argValue">Value of the argument to print</param>
        /// <param Name="format">Format to print argument in.  Must have string.Format() syntax</param>
        /// <returns>Formatted string, or "" if arg value is null</returns>
        private void WriteArgIfValid(object argValue, string format)
        {
            if (argValue == null)
                return;

            this.stream.WriteLine(format, argValue);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param Name="stream">stream to write to.  the stream should already be configured and open</param>
        /// <param Name="testParameters">object to retrieve test settings from</param>
        public DefaultLogProvider(TextWriter stream, string name)
        {
            this.stream = stream;
            LogProviderName = name;
        }

        /// <summary>
        /// This is the user friendly name for the log provider that is being loaded.
        /// </summary>
        public string LogProviderName
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Splits up a line that has \n characters in it
        /// </summary>
        /// <param name="message">The input line</param>
        /// <returns>An array of lines</returns>
        private string[] SplitLineFeed(string message)
        {
            if (message == null)
            {
                return new string[] { "" };
            }

            string[] lines = message.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();
            }

            return lines;
        }

        /// <summary>
        /// Disposes this object and all of its disposable members
        /// </summary>
        public void Dispose()
        {
            // dispose members
            if (this.stream != null)
            {
                this.stream.Close();
                this.stream = null;
            }
        }


        /// <summary>
        /// Called when logging begins.  No other methods should be called before this one.
        /// </summary>
        public void StartLogger()
        {
        }

        private void WriteDivider(char devChar ='*')
        {
            int repeat = 90;

            if (this.stream == Console.Out)
            {
                repeat = Console.WindowWidth;
            }

            for (int i = 0; i < repeat - 1; ++i)
                this.stream.Write(devChar);
            this.stream.WriteLine();
        }

        /// <summary>
        /// This function outputs most of the useful test information at the beginning of the scenario.
        /// </summary>
        public void StartScenario(IScenario scenario)
        {
            WriteDivider();
            this.stream.Write("SCENARIO:");
            if (this.stream == Console.Out)
                Console.ForegroundColor = ConsoleColor.Cyan;

            this.stream.Write(scenario.Name);
            this.stream.WriteLine();

            if (this.stream == Console.Out)
                Console.ResetColor();

            //foreach (string parameter in RuntimeParameters.Instance.CommandLineParameters)
            //{
            //    this.stream.WriteLine("    {0}", parameter);
            //}
            this.stream.WriteLine(" -id " + scenario.Assembly + ":" + scenario.Factory + ":" + scenario.Name);
            this.stream.WriteLine("START TIME: {0:yyyy-MM-dd HH:mm:ss}", InternalLogger.Instance.ScenarioStartTime);
            WriteDivider();
            this.stream.WriteLine();
            this.stream.WriteLine();
        }

        /// <summary>
        /// Adds specified file to log. Ignored by the TextStreamLogProvider
        /// </summary>
        /// <param name="fileName">Full path to file to add to list</param>
        public void LogFile(string fileName)
        {
        }


        /// <summary>
        /// Called when a result is logged for a test (within Start/EndTest).  No result is logged until EndTest,
        /// so this should be implemented to simply report the subresult appropriately.
        /// </summary>
        /// <param Name="result">Result to log</param>
        /// <param Name="message">Text associated with result</param>
        public void LogSubResult(Result result, Exception ex, string message)
        {
            string[] lines = SplitLineFeed(message);

            // Write the result and first line of message
            this.WriteResult("    ", result, lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                this.stream.WriteLine("           {0}", lines[i]);
            }

            while (ex != null)
            {
                this.stream.WriteLine("    !      -----------------------------------");
                this.stream.WriteLine("    !      exception message:");
                this.stream.WriteLine("    !      {0}", ex.Message);
                this.stream.WriteLine("    !      exception type:");
                this.stream.WriteLine("    !      {0}", ex.GetType());
                lines = SplitLineFeed(ex.StackTrace);
                for (int i = 0; i < lines.Length; i++)
                {
                    this.stream.WriteLine("    !      {0}", lines[i]);
                }
                this.stream.WriteLine("    !      -----------------------------------");

                // Move on to the inner exception, if there is one.
                if (ex.InnerException != null)
                {
                    this.stream.WriteLine("    !      Inner exception:");
                }

                ex = ex.InnerException;
            }
        }

        /// <summary>
        /// Writes a line of debug text
        /// </summary>
        /// <param Name="message">Line of text to write</param>
        public void WriteLine(string message)
        {
            string[] lines = SplitLineFeed(message);

            for (int i = 0; i < lines.Length; i++)
            {
                this.stream.WriteLine("    +      {0}", lines[i]);
            }
        }

        /// <summary>
        /// Writes a line of debug text of specified type
        /// </summary>
        /// <param Name="messageType">Metadata tag for this message</param>
        /// <param Name="message">Line of text to write</param>
        public void WriteLine(string messageType, string message)
        {
            this.WriteLine(message);
        }

        /// <summary>
        /// Writes out an exception.  The default behavior if this function is not overridden is to 
        /// pipe the info into WriteError().  Like WriteError, this funciton can be called before
        /// StartLogger, so watch out.
        /// </summary>
        /// <param name="ex">The Exception object.  This will tell where the exception occurred and what kind of exception there was.</param>
        /// <param name="errorMessage">This should tell what was going on when the exception was caught.</param>
        public void WriteException(Exception ex, string errorMessage)
        {
            string[] lines = SplitLineFeed(errorMessage);

            if (null == this.stream)
                return;

            for (int i = 0; i < lines.Length; i++)
            {
                this.stream.WriteLine("    !      {0}", lines[i]);
            }

            // Write out exception info, if it exists
            while (ex != null)
            {
                this.stream.WriteLine("    !      -----------------------------------");
                this.stream.WriteLine("    !      {0}", ex.Message);

                lines = SplitLineFeed(ex.StackTrace);
                for (int i = 0; i < lines.Length; i++)
                {
                    this.stream.WriteLine("    !      {0}", lines[i]);
                }
                this.stream.WriteLine("    !      -----------------------------------");

                // Move on to the inner exception, if there is one.
                if (ex.InnerException != null)
                {
                    this.stream.WriteLine("    !      Inner exception:");
                }

                ex = ex.InnerException;
            }
        }

        /// <summary>
        /// Writes a URL out to the log files.  Ignored by this log provider.
        /// </summary>
        /// <param name="description">A description that comes between the &lt;a&gt; tags</param>
        /// <param name="link">The target of the link</param>
        /// <param name="copyFile">True to copy linked file (WTT log)</param>
        public void WriteUrlLink(string description, string link, bool copyFile)
        {
        }

        /// <summary>
        /// This function is ignored by the provider.
        /// </summary>
        /// <param name="filename"></param>
        public void LogScreenShot(string filename)
        {
        }

        /// <summary>
        /// This function is called when all of the tests in TestParameters.TestIDs have been run and the scenario is over.
        /// </summary>
        public void StopScenario()
        {
            string elapsed = InternalLogger.Instance.ScenarioElapsedTime.TotalMinutes.ToString("0.00");

            WriteDivider('-');
            this.stream.WriteLine("END TIME: {0:yyyy-MM-dd HH:mm:ss}", InternalLogger.Instance.CurrentTime);
            this.stream.WriteLine("ELAPSED:  {0} minutes.", elapsed);
            WriteDivider('-');
        }

        /// <summary>
        /// Called when logging is complete.  No other methods should be called after this one.
        /// </summary>
        public void StopLogger()
        {
            // If logging a summary
            if (InternalLogger.Instance.LogResultSummary != LogResults.None)
            {
                // Write the final results summary
                this.WriteResultSummary();
            }

            //this.stream.Close();
            //this.stream = null;
        }

        /// <summary>
        /// Write result summary to the stream
        /// </summary>
        private void WriteResultSummary()
        {
            Result finalResult = Result.NoResult;

            // No sense in a summary if there's only one result
            //if ((Logger.Default.LogResultSummary == LogResults.FailsOnly))
            {
                WriteDivider();
                this.stream.WriteLine("Scenario Summaries");
                this.stream.WriteLine();

                Dictionary<string, Dictionary<string, Result>> rollupIdToDescriptionToResults = new Dictionary<string, Dictionary<string, Result>>();
                Dictionary<string, Dictionary<string, List<string>>> failedMessages = new Dictionary<string, Dictionary<string, List<string>>>();

                // Write results summary
                foreach (var key in InternalLogger.Instance.LoggedResults)
                {
                    foreach (LogResult logResult in key.Value)
                    {

                        if (logResult.RollupResult == Result.InfrastructureWarning)
                            continue;

                        string descr = null == logResult.Description ? string.Empty : logResult.Description;
                        string id = null == logResult.ScenarioDisplayID ? string.Empty : logResult.ScenarioDisplayID;
                        if (!rollupIdToDescriptionToResults.ContainsKey(id))
                        {
                            rollupIdToDescriptionToResults.Add(id, new Dictionary<string, Result>());
                            failedMessages.Add(logResult.ScenarioDisplayID, new Dictionary<string, List<string>>());
                            //failedMessages.Add(logResult.TestScenario.LocalScenarioId, new List<string>());
                            //rollupDescriptions.Add(logResult.TestScenario.LocalScenarioId, logResult.Scenario.Description);
                        }

                        if (!rollupIdToDescriptionToResults[id].ContainsKey(descr.ToLowerInvariant()))
                        {
                            rollupIdToDescriptionToResults[id].Add(descr.ToLowerInvariant(), Result.NoResult);
                            failedMessages[id].Add(descr.ToLowerInvariant(), new List<string>());
                        }

                        // Write result and cooresponding message
                        if (logResult.RollupResult != Result.Pass)
                        {
                            failedMessages[id][descr.ToLowerInvariant()].Add(logResult.ResultMessage);
                        }

                        if (rollupIdToDescriptionToResults[id][descr.ToLowerInvariant()] < logResult.RollupResult)
                        {
                            rollupIdToDescriptionToResults[id][descr.ToLowerInvariant()] = logResult.RollupResult;
                        }

                        // Track highest result
                        if (finalResult < logResult.RollupResult)
                        {
                            finalResult = logResult.RollupResult;
                        }
                    }
                }

                int scenarioCount = 0;
                foreach (var localScenarioID in rollupIdToDescriptionToResults.Keys)
                {
                    ++scenarioCount;
                    foreach (var description in rollupIdToDescriptionToResults[localScenarioID].Keys)
                    {
                        Result localResult = rollupIdToDescriptionToResults[localScenarioID][description];
                        List<string> messages = failedMessages[localScenarioID][description];

                        if (!String.IsNullOrEmpty(description))
                            this.WriteResult("".PadLeft(10), localResult, " : " + localScenarioID + ": " + description);
                        else
                            this.WriteResult(scenarioCount.ToString("0000") + "".PadLeft(6), localResult, " : " + localScenarioID);

                        foreach (string err in messages)
                            this.WriteLine("     >" + err);
                    }
                }

                // Write final result
                this.stream.WriteLine();
                this.stream.Write("Overall Result : ");
                this.WriteResultInColor(finalResult);
                this.stream.WriteLine();
                WriteDivider();
            }
        }

        /// <summary>
        /// Write result string in correct color
        /// </summary>
        /// <param name="result">Test result</param>
        private void WriteResult(string prefix, Result result, string postfix)
        {
            // Write before result
            if (!String.IsNullOrEmpty(prefix))
            {
                this.stream.Write(prefix);
            }

            // Write result
            this.WriteResultInColor(result);

            // Spacing to line up postfix
            this.stream.Write("  {0}", result.ToString().Length < 5 ? " " : "");

            // Write after result
            if (!String.IsNullOrEmpty(postfix))
            {
                this.stream.Write(postfix);
            }

            this.stream.WriteLine();
        }

        /// <summary>
        /// Write result string in correct color
        /// </summary>
        /// <param name="result">Test result</param>
        private void WriteResultInColor(Result result)
        {
            if (this.stream == Console.Out)
            {
                switch (result)
                {
                    case Result.Exception:
                    case Result.PerfWarning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case Result.ProductException:
                    case Result.Fail:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case Result.InfrastructureWarning:
                    case Result.Warning:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case Result.Pass:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }

                this.stream.Write(result.ToString());
                Console.ResetColor();
            }
            else
            {
                this.stream.Write(result.ToString());
            }
        }
    }
}
