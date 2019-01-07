using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mossharbor.ScenarioTestFramework
{
    /// <summary>
    /// Store test name, message and result
    /// </summary>
    public class LogResult
    {
        /// <summary>
        /// Scenario this test was run in
        /// </summary>
        private IScenario scenario;

        /// <summary>
        /// Result for this test
        /// </summary>
        private Result rollupResult;

        /// <summary>
        /// Test message for this result
        /// </summary>
        private string resultMessage;

        /// <summary>
        /// This is the display string for the current scenario.
        /// </summary>
        private string scenarioDisplayID;

        /// <summary>
        /// This is the description of the test that logged the result
        /// </summary>
        private string testDescription;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenario">Scenario</param>
        /// <param name="result">Scenario sub result</param>
        /// <param name="message">Scenario sub result message</param>
        public LogResult(IScenario scenario, Result result, string message)
        {
            this.testDescription = scenario.Description;
            this.scenario = scenario;
            this.rollupResult = result;
            this.resultMessage = message;
            this.scenarioDisplayID = scenario.Name;
        }

        /// <summary>
        /// Scenario this test was run in
        /// </summary>
        public IScenario Scenario
        {
            get { return this.scenario; }
        }

        public string ScenarioDisplayID
        {
            get
            {
                return this.Scenario.Name;
            }
        }

        /// <summary>
        /// This is the description of the test case that logged this result.
        /// </summary>
        public string Description
        {
            get { return Scenario.Description; }
        }

        /// <summary>
        /// Result for this scenario
        /// </summary>
        public Result RollupResult
        {
            get { return this.rollupResult; }
        }

        /// <summary>
        /// Message for this result
        /// </summary>
        public string ResultMessage
        {
            get { return this.resultMessage; }
        }
    }
}
