using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Mossharbor.ScenarioTestFramework
{
    public class XmlSummaryLogger : ILogProvider, IDisposable
    {
        private string logFile;

        public XmlSummaryLogger(string logFile)
        {
            this.logFile = logFile;
        }

        public string LogProviderName
        {
            get
            {
                return "XmlSummaryLogger";
            }
        }

        public void Dispose()
        {
        }

        public void LogFile(string fileName)
        {
        }

        public void LogScreenShot(string screenShotFileName)
        {
        }

        public void LogSubResult(Result result, Exception ex, string message)
        {
        }

        public void StartLogger()
        {
        }

        public void StartScenario(IScenario scenario)
        {
        }

        public void StopLogger()
        {
            // If logging a summary
            if (InternalLogger.Instance.LogResultSummary != LogResults.None)
            {
                // Write the final results summary
                this.WriteResultSummary();
            }
        }

        private void WriteResultSummary()
        {
            XmlDocument xmlDoc = new XmlDocument();
            Result finalResult = Result.NoResult;
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
            XmlElement summaryElement = xmlDoc.CreateElement("Summary");
            summaryElement.SetAttribute("Final", finalResult.ToString());
            xmlDoc.AppendChild(summaryElement);

            int scenarioCount = 0;
            foreach (var localScenarioID in rollupIdToDescriptionToResults.Keys)
            {
                XmlElement testSummary = xmlDoc.CreateElement("Scenario");
                summaryElement.AppendChild(testSummary);

                ++scenarioCount;
                foreach (var description in rollupIdToDescriptionToResults[localScenarioID].Keys)
                {
                    Result localResult = rollupIdToDescriptionToResults[localScenarioID][description];
                    List<string> messages = failedMessages[localScenarioID][description];

                    testSummary.SetAttribute("Index", scenarioCount.ToString("0000"));
                    testSummary.SetAttribute("Result", localResult.ToString());
                    testSummary.SetAttribute("Id", localScenarioID);
                    testSummary.SetAttribute("Description", description);

                    if (messages.Count != 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (string err in messages)
                            sb.AppendLine(err);

                        XmlElement errorElement = xmlDoc.CreateElement("Error");
                        testSummary.AppendChild(errorElement);
                        XmlText errorText = xmlDoc.CreateTextNode(sb.ToString());
                        errorElement.AppendChild(errorText);
                    }
                }
            }

            xmlDoc.Save(this.logFile);
        }

        public void StopScenario()
        {
        }

        public void WriteException(Exception ex, string errorMessage)
        {
        }

        public void WriteLine(string message)
        {
        }

        public void WriteLine(string messageType, string message)
        {
        }

        public void WriteUrlLink(string description, string link, bool copyFile)
        {
        }
    }
}
