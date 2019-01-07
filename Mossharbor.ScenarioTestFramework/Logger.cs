using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mossharbor.ScenarioTestFramework
{
    public class Logger
    {
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
        public Result CurrentResult
        {
            get
            {
                return InternalLogger.Instance.CurrentSubResult;
            }
            set
            {
                InternalLogger.Instance.CurrentSubResult = value;
            }
        }

        /// <summary>
        /// This function logs a result for the test case.
        /// </summary>
        /// <param name="result">This is the result to log. <see cref="TestManagement.Result"/></param>
        /// <param name="output">This is the output string attached to the result.</param>
        /// <param name="arguments">This is a list of objects whose .ToString() function will be put into the
        /// appropriate place in the output string.</param>
        /// <remarks>
        /// This is equivalent to calling <seealso cref="LogSubResult"/> of the Logger.
        /// <seealso cref="Result"/>
        /// </remarks>
        public static void LogResult(Result result, string output, params object[] arguments)
        {
            InternalLogger.Instance.LogSubResult(result, output, arguments);
        }

        /// <summary>
        /// This function logs a result for the test case.
        /// </summary>
        /// <param name="result">This is the result to log. <see cref="TestManagement.Result"/></param>
        /// <param name="output">This is the output string attached to the result.</param>
        /// <remarks>
        /// This is equivalent to calling <seealso cref="LogSubResult"/> of the Logger.
        /// <seealso cref="Result"/>
        /// THis function is added for compatability with MC++
        /// </remarks>
        public static void LogResult(Result result, string output)
        {
            InternalLogger.Instance.LogSubResult(result, output, null);
        }

        public static void LogPass(string output)
        {
            LogResult(Result.Pass, output);
        }

        public static void LogFail(string output)
        {
            LogResult(Result.Fail, output);
        }

        public static void LogIf(bool pass, string passString, string failString)
        {
            if (pass)
                LogPass(passString);
            else
                LogFail(failString);
        }


        public static bool LogFileEquivalence(bool expectMatch, string file1, string file2)
        {
            System.Security.Cryptography.SHA1 hasher = System.Security.Cryptography.SHA1.Create();
            byte[] file1hash = null;
            byte[] file2hash = null;

            using (Stream file1strm = File.OpenRead(file1))
            {
                using (Stream file2strm = File.OpenRead(file2))
                {
                    file1hash = hasher.ComputeHash(file1strm);
                    file2hash = hasher.ComputeHash(file2strm);
                }
            }

            if (null == file1hash || null == file2hash)
            {
                LogFail("The two files did not hash correctly.");
                return false;
            }

            if (file1hash.Length != file2hash.Length)
            {
                LogFail("The two files did not hash correctly.");
                return false;
            }

            bool allMatched = true;
            for (int i = 0; i < file1hash.Length; ++i)
            {
                if (expectMatch && file1hash[i] != file2hash[i])
                {
                    LogFail(Path.GetFileName(file1) + " != " + Path.GetFileName(file2));
                    return false;
                }
                else if (!expectMatch && file1hash[1] != file2hash[i])
                {
                    allMatched = false;
                }
            }

            if (!expectMatch && allMatched)
                LogFail(Path.GetFileName(file1) + " == " + Path.GetFileName(file2));
            else if (expectMatch)
                LogPass(Path.GetFileName(file1) + " == " + Path.GetFileName(file2));
            else
                LogPass(Path.GetFileName(file1) + " != " + Path.GetFileName(file2));

            return true;
        }

        /// <summary>
        /// This is the function that is used to put generic output to the logging routine.
        /// </summary>
        /// <remarks>
        /// This is equivalent to calling <seealso cref="WriteLine"/> of the Logger.
        /// </remarks>
        /// <param name="output">This is the text to write to the log.</param>
        public static void WriteLine(string output, params object[] arguments)
        {
            InternalLogger.Instance.WriteLine(output, arguments);
        }

        /// <summary>
        /// This is the function that is used to put generic output to the logging routine.
        /// </summary>
        /// <param name="output">This is the text that we wish to output.</param>
        /// <remarks>
        /// This is equivalent to calling <seealso cref="WriteLine"/> of the Logger.
        /// </remarks>
        public static void WriteLine(string output)
        {
            InternalLogger.Instance.WriteLine(output);
        }

        /// <summary>
        /// This is the function that is used to put generic output to the logging routine.
        /// </summary>
        /// <param name="messageHighlightType">This is the log highlighting type</param>
        /// <param name="output">This is the text that we wish to output</param>
        /// <param name="arguments">Output format string arguments</param>
        public static void WriteLine(MessageHighlightType messageHighlightType, string output, params object[] arguments)
        {
            InternalLogger.Instance.WriteLine(messageHighlightType, output, arguments);
        }

        /// <summary>
        /// This is the function that is used to put generic output to the logging routine.
        /// </summary>
        /// <param name="messageHighlightType">This is the log highlighting type</param>
        /// <param name="output">This is the text that we wish to output.</param>
        public static void WriteLine(MessageHighlightType messageHighlightType, string output)
        {
            InternalLogger.Instance.WriteLine(messageHighlightType, output);
        }
    }
}
