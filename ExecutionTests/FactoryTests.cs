using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExecutionTests
{
    using System.Reflection;
    using ScenarioExecutionFramework;

    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void VerifyBasicAddForCustomBuilder()
        {
            try
            {
                ScenarioManager.Instance.ExecuteScenario("ExecutionTests", "CustomBuilderFactory", "VerifyBasicAddForCustomBuilder");
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void TestWeSearchAllAssemblies()
        {
            try
            {
                ScenarioManager.Instance.ExecuteScenario("ExecutionTests", "VerifyBasicAddForCustomBuilder");
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void VerifyBasicCustomFactoryAdd()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "BasicCustomFactory");
                ScenarioManager.Instance.LoadScenario("VerifyBasicAdd");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void VerifyAddBasicCustomFactoryBasicSubScenario()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "BasicCustomFactory");
                ScenarioManager.Instance.LoadScenario("VerifyAddBasicSubScenario");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun,1);
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun,0);}
        }

        [TestMethod]
        public void VerifyBasicQuickFactoryAdd()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "BasicQuickFactory");
                ScenarioManager.Instance.LoadScenario("VerifyBasicAdd");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
                ScenarioManager.Instance.Clear();
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void VerifyAddBasicQuickFactoryBasicSubScenario()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "BasicQuickFactory");
                ScenarioManager.Instance.LoadScenario("VerifyAddBasicSubScenario");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
                ScenarioManager.Instance.Clear();
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void VerifyBasicSimpleFactoryAdd()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "SimpleTestFactory");
                ScenarioManager.Instance.LoadScenario("VerifyBasicAdd");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
                ScenarioManager.Instance.Clear();
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void VerifyAddBasicSimpleFactoryBasicSubScenario()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "SimpleTestFactory");
                ScenarioManager.Instance.LoadScenario("VerifyAddBasicSubScenario");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 1);
                ScenarioManager.Instance.Clear();
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }

        [TestMethod]
        public void VerifyAddBasicSimpleFactoryTwoScenarios()
        {
            try
            {
                ScenarioManager.Instance.LoadFactory("ExecutionTests", "SimpleTestFactory");
                ScenarioManager.Instance.LoadScenario("TwoScenarios");
                ScenarioManager.Instance.ExecuteScenarios();
                Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 2);
                ScenarioManager.Instance.Clear();
            }
            finally { ScenarioManager.Instance.Clear(); Assert.AreEqual(ScenarioManager.Instance.ScenariosRun, 0); }
        }
    }
}
