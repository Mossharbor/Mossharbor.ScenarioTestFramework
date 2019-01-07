using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    public interface ILogProvider
    {
        /// <summary>
        /// Adds specified file to log.  It is up to the log providers to determine what to do with it.
        /// </summary>
        /// <param name="fileName">Full path to file to add to list</param>
        void LogFile(string fileName);

        /// <summary>
        /// THis is the user friendly name for the log provider that is being loaded.
        /// </summary>
        string LogProviderName { get; }

        /// <summary>
        /// Called when needed to create screenshot
        /// </summary>
        /// <param name="screenShotFileName">the name for the screenshot</param>
        void LogScreenShot(string screenShotFileName);

        /// <summary>
        /// Called when a result is logged for a scenario (within Start/End).  No result is logged until SCenarioEnd,
        /// so this should be implemented to simply report the subresult appropriately.
        /// </summary>
        /// <param Name="result">Result to log</param>
        /// <param name="ex">An exception that occurred (can be null)</param>
        /// <param Name="message">Text associated with result</param>
        void LogSubResult(Result result, Exception ex, string message);

        /// <summary>
        /// Called when logger is first initialized.  No other methods should be called before this one.
        /// </summary>
        void StartLogger();

        /// <summary>
        /// This function is called when the scenario starts.
        /// </summary>
        void StartScenario(IScenario scenario);

        /// <summary>
        /// Called when all sceanrios have been completed.  No other methods will be called after this one.
        /// </summary>
        void StopLogger();

        /// <summary>
        /// This function is called when all of the scenarios have been run and the scenario is over.
        /// </summary>
        void StopScenario();

        /// <summary>
        /// Writes out an exception.  
        /// </summary>
        /// <param name="ex">The Exception object.  This will tell where the exception occurred and what kind of exception there was.</param>
        /// <param name="errorMessage">This should tell what was going on when the exception was caught.</param>
        /// <remarks>This funciton can be called before
        /// StartLogger, so watch out.</remarks>
        void WriteException(Exception ex, string errorMessage);

        /// <summary>
        /// Writes a line of debug text
        /// </summary>
        /// <param Name="message">Line of text to write</param>
        void WriteLine(string message);

        /// <summary>
        /// Writes a line of debug text of specified type
        /// </summary>
        /// <param name="messageType">Metadata tag for this message</param>
        /// <param name="message">Line of text to write</param>
        /// <remarks>
        /// The message type was added for the xml log provider to parse specific messages
        /// This will log debug messages as a type other than "Info" or "Error"
        /// Other log providers can use this for their own message types, as long as they
        /// have the ability to ignore unknown types
        /// </remarks>
        void WriteLine(string messageType, string message);

        /// <summary>
        /// Writes a URL out to the log files
        /// </summary>
        /// <param name="description">A description that comes between the &lt;a&gt; tags</param>
        /// <param name="link">The target of the link</param>
        /// <param name="copyFile">True to copy linked file (WTT log)</param>
        void WriteUrlLink(string description, string link, bool copyFile);
    }
}
