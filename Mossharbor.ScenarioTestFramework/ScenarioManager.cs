using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.IO;
using Mossharbor.ScenarioTestFramework.Exceptions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Mossharbor.ScenarioTestFramework
{
    public class ScenarioManager
    {
        private List<IScenarioFactory> factories = new List<IScenarioFactory>();
        private ConcurrentBag<string> scenarioIds = new ConcurrentBag<string>();

        public void LoadFactory(string assembly, string factoryName = null)
        {
            // Load the assembly (if provided) otherwise all assemblies
            Assembly scenarioAssembly = LoadAssembly(assembly);

            if (null == scenarioAssembly)
                throw new CouldNotFindScenarioFactoryAssemblyException(assembly);

            // Load the factory (if provided) otherwise all factories
            LoadScenarioFactory(factoryName, scenarioAssembly);
        }

        public void LoadScenario(string scenarioName)
        {
            int execLoops = Math.Max(1, RuntimeParameters.Instance.ExecutionLoops);

            for(int i=0; i < execLoops; ++i)
                scenarioIds.Add(scenarioName);
        }

        private IScenario[] GetScenariosFrom(string scenarioName)
        {
            // Ask the factory if they can provide a scenario build for this scenario.
            List<IScenario> scenarios = new List<IScenario>();
            foreach (IScenarioFactory factory in factories)
            {
                IScenario[] newScenarios = factory.GetScenarios(scenarioName);

                if (null != newScenarios && 0 != newScenarios.Length)
                {
                    foreach (IScenario scen in newScenarios)
                    {
                        scen.Factory = factory.FactoryName;
                        scen.Assembly = factory.Assembly;
                        if (String.IsNullOrEmpty(scen.Name))
                            scen.Name = scenarioName;
                        scenarios.Add(scen);
                    }
                }
            }

            return scenarios.ToArray();
        }

        public int ScenariosRun
        {
            get;
            set;
        }

        public ConcurrentBag<string> ScenarioIds { get { return scenarioIds; } internal set { scenarioIds = value; } }

        public int IdCount { get { return scenarioIds.Count; } }

        private ScenarioManager()
        {
            ScenariosRun = 0;
        }

        public void ExecuteScenarios()
        {
            TaskFactory taskFactory = new TaskFactory();
            InternalLogger.Instance.StartLogger();
            List<IScenario> scenariosRun = new List<IScenario>();

            //foreach (string scenarioName in scenarioIds)
            int initialIDCount = scenarioIds.Count;
            for (int idIndex = 0; idIndex < initialIDCount; ++idIndex)
            {
                string scenarioName = scenarioIds.ElementAt(idIndex);
                IScenario[] scenarios = GetScenariosFrom(scenarioName);
                //foreach (var scenario in scenarios)
                for(int s=0; s < scenarios.Length; ++s)
                {
                    ScenariosRun++;
                    var scenario = scenarios[s];
                    scenariosRun.Add(scenario);
                    InternalLogger.Instance.StartScenario(scenario);

                    int scenarioExecuteTimeout = RuntimeParameters.Instance.OverrideExecuteTimeout > 0 ? RuntimeParameters.Instance.OverrideExecuteTimeout : scenario.ExecuteTimeout;
                    int scenarioCleanupTimeout = RuntimeParameters.Instance.OverrideCleanupTimeout > 0 ? RuntimeParameters.Instance.OverrideCleanupTimeout : scenario.CleanupTimeout;
                    int scenarioSetupTimeout = RuntimeParameters.Instance.OverrideSetupTimeout > 0 ? RuntimeParameters.Instance.OverrideSetupTimeout : scenario.SetupTimeout;

                    int sceanrioLoops = 1 == RuntimeParameters.Instance.ScenarioLoops ? scenario.ScenarioLoops : scenario.ScenarioLoops * RuntimeParameters.Instance.ScenarioLoops;
                    for (int iteration = 0; iteration < sceanrioLoops; ++iteration)
                    {
                        bool setupAttempted = false;
                        try
                        {
                            if (!RuntimeParameters.Instance.NoSetup)
                                WaitOnTask(taskFactory, scenario, scenarioCleanupTimeout, new SetupTimeoutExceptionException(scenario.Name), () => scenario.SetUp());
                            else
                                InternalLogger.Instance.WriteLine("Skipping Setup as specified on the command line");

                            setupAttempted = true;

                            for (int i = 0; i < Math.Max(1, scenario.ExecutionLoops) ; ++i)
                            {
                                WaitOnTask(taskFactory, scenario, scenarioExecuteTimeout, new ExecuteTimeoutException(scenario.Name), () => scenario.Execute());
                            }
                        }
                        catch (ExecuteTimeoutException ex)
                        {
                            InternalLogger.Instance.WriteException(Result.Timeout, ex, "Scenario execute timed out {0}", scenario.Name);
                        }
                        catch (SetupTimeoutExceptionException ex)
                        {
                            InternalLogger.Instance.WriteException(Result.Timeout, ex, "Scenario setup timed out {0}", scenario.Name);
                        }
                        catch (Exception e)
                        {
                            Exception extoLog = (e is AggregateException) ? e.InnerException : e;
                            string exType = extoLog.GetType().ToString();
                            if (setupAttempted)
                            {
                                string message = "{1}: {0}";
                                if (scenario.ExceptionAreProductExceptions)
                                {
                                    InternalLogger.Instance.WriteException(Result.ProductException, extoLog, message, extoLog.Message, exType);
                                    InternalLogger.Instance.LogSubResult(Result.ProductException, message, extoLog.Message, exType);
                                }
                                else
                                {
                                    InternalLogger.Instance.WriteException(Result.Exception, extoLog, message, extoLog.Message, exType);
                                    InternalLogger.Instance.LogSubResult(Result.Exception, message, extoLog.Message, exType);
                                }
                            }
                            else
                            {
                                string message = "SetupFailed: {1}: {0}";
                                InternalLogger.Instance.WriteException(Result.Exception, extoLog, message, extoLog.Message, exType);
                                InternalLogger.Instance.LogSubResult(Result.Exception, message, extoLog.Message, exType);
                            }
                            
                            //if (ex is AggregateException)
                            //    throw ex.InnerException;
                            //throw;
                        }
                        finally
                        {
                            try
                            {
                                if (!RuntimeParameters.Instance.NoCleanup)
                                    WaitOnTask(taskFactory, scenario, scenarioCleanupTimeout, new CleanupTimeoutException(scenario.Name), () => scenario.CleanUp());
                                else
                                    InternalLogger.Instance.WriteLine("Skipping scenario cleanup as specified on command line");
                            }
                            catch (CleanupTimeoutException ex)
                            {
                                InternalLogger.Instance.WriteException(Result.Timeout, ex, "Scenario cleanup timedout {0}", scenario.Name);
                            }
                            catch (Exception ex)
                            {
                                string message = "Failed to execute scenario {0}";
                                InternalLogger.Instance.WriteException(Result.Exception, ex, message, scenario.Name);
                            }
                        }
                    }

                    InternalLogger.Instance.StopScenario(scenario);
                } // scenario in list
            }//scenario name

            foreach (IScenario scenario in scenariosRun)
            {
                try
                {
                    int scenarioCleanupTimeout = RuntimeParameters.Instance.OverrideCleanupTimeout > 0 ? RuntimeParameters.Instance.OverrideCleanupTimeout : scenario.CleanupTimeout;

                    if (!RuntimeParameters.Instance.NoCleanup)
                        WaitOnTask(taskFactory, scenario, scenarioCleanupTimeout, new CleanupTimeoutException(scenario.Name), () => scenario.EndRunCleanup());
                    else
                        InternalLogger.Instance.WriteLine("Skipping end of all scenarios cleanup as specified on command line");
                }
                catch (Exception ex)
                {
                    InternalLogger.Instance.WriteException(Result.Exception, ex, "Failed to EndRunCleanup for scenario {0}",scenario.Name);
                }
            }

            InternalLogger.Instance.StopLogger();
        }

        public void ExecuteScenario(string assembly, string factory, string scenarioName)
        {
            if (!String.IsNullOrEmpty(factory) && !String.IsNullOrEmpty(assembly))
            {
                Assembly scenarioAssembly = LoadAssembly(assembly);

                if (null == scenarioAssembly)
                    throw new CouldNotFindScenarioFactoryAssemblyException(assembly);

                LoadScenarioFactory(factory, scenarioAssembly);
            }
            else if (!String.IsNullOrEmpty(assembly))
                ScenarioManager.Instance.LoadFactory(assembly);
            else
                throw new CouldNotFindScenarioFactoryAssemblyException();

            ScenarioManager.Instance.LoadScenario(scenarioName);
            ScenarioManager.Instance.ExecuteScenarios();
        }

        public void ExecuteScenario(string assembly, string scenarioName)
        {
            ExecuteScenario(assembly, null, scenarioName);
        }

        public void ExecuteScenario(IScenarioFactory factory, string scenarioName)
        {
            scenarioIds = new ConcurrentBag<string>();
            factories.Clear();
            AddFactory(factory);
            LoadScenario(scenarioName);
            ExecuteScenarios();
        }

        public void ExecuteScenario(string scenarioName)
        {
            scenarioIds = new ConcurrentBag<string>();
            LoadScenario(scenarioName);
            ExecuteScenarios();
        }

        public void Clear()
        {
            factories.Clear();
            scenarioIds = new ConcurrentBag<string>();
            ScenariosRun = 0;
        }

        internal static void WaitOnTask(TaskFactory taskFactory, IScenario scenario, int timeoutInSeconds, Exception ex,Action toWaitOn)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                toWaitOn();
                return;
            }

            Task waitTask = taskFactory.StartNew(toWaitOn);

            if (!waitTask.Wait(TimeSpan.FromSeconds(timeoutInSeconds)))
            {
                if (!Debugger.IsAttached)
                    throw ex;
                else
                {
                    System.Diagnostics.Debug.Assert(false);
                    waitTask.Wait();
                }
            }
        }

        ///<summary>
        ///Gets the Scenario singleton.
        ///</summary>
        public static ScenarioManager Instance
        {
            get
            {
                return SingletonScenarioInstance.Instance;
            }
        }

        /// <summary>
        /// Gets namespace
        /// </summary>
        private string Namespace
        {
            get { return this.GetType().Namespace; }
        }

        /// <summary>
        /// Loads an Assembly containing a IScenarioBuilderFactory
        /// </summary>
        /// <param name="assemblyName">assembly to load</param>
        /// <returns>instance of that assembly</returns>
        private Assembly LoadAssembly(string assemblyName)
        {
            Assembly assembly = null;

            // verify we have an extension specified, if not, add it.
            string assemblyExtension = String.Empty;
            try
            {
                assemblyExtension = Path.GetExtension(assemblyName);
            }
            catch
            {
                // no extension, stick with ""
            }

            string assemblyFileName = assemblyName;
            if (!assemblyExtension.Equals("dll", StringComparison.InvariantCultureIgnoreCase) &
                !assemblyExtension.Equals("exe", StringComparison.InvariantCultureIgnoreCase))
            {
                assemblyFileName += ".dll";
            }

            // make sure the assembly is present
            string assemblyLocation = Path.Combine(System.Environment.CurrentDirectory, assemblyFileName);
            if (!File.Exists(assemblyLocation))
            {
                // check if we're using the short name
                assemblyName = String.Format("{0}.{1}", this.Namespace, assemblyName);
                assemblyLocation = Path.Combine(System.Environment.CurrentDirectory, assemblyFileName);
                if (!File.Exists(assemblyLocation))
                {
                    throw new CannotFindScenarioAssemblyException(string.Format("Unable to locate module \"{0}\".", assemblyLocation));
                }
            }

            // dynamically load assembly containing condition
            try
            {
                // by full path
                assembly = Assembly.LoadFrom(assemblyLocation);
            }
            // check for any missing dependencies for the condition
            catch (FileLoadException loadEx)
            {
                throw new CannotLoadScenarioAssemblyException(string.Format("Unable to load module \"{0}\".", assemblyLocation), loadEx);
            }
            catch
            {
                assembly = null;
            }

            if (assembly == null)
            {
                try
                {
                    // or just assembly name
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    assembly = null;
                }
            }

            if (assembly == null)
            {
                throw new CannotLoadScenarioAssemblyException(string.Format("Unable to load module \"{0}\".", assemblyLocation));
            }

            return assembly;
        }

        /// <summary>
        /// gets an instance of the requested ConditionBase class by name
        /// </summary>
        /// <param name="factoryName">name of the requested ConditionBase class</param>
        /// <param name="assembly">refernce to assembly containing requested ConditionBase class</param>
        /// <returns>instance of requested condition</returns>
        private void LoadScenarioFactory(string factoryName, Assembly assembly)
        {
            // Find something that inherits from ScenarioFactoryBase
            IScenarioFactory factory = null;
            var factoryType = Type.GetType(String.Format("{0}.{1},{0}", this.Namespace, typeof(IScenarioFactory).Name));
            int countInitial = factories.Count;

            foreach (Type type in assembly.GetTypes())
            {
                if (null != type.Namespace && type.Namespace.ToLowerInvariant().StartsWith(this.Namespace.ToLowerInvariant()))
                    continue;

                foreach (var interf in type.GetInterfaces())
                {
                    if (interf.Equals(factoryType))
                    {
                        factory = System.Activator.CreateInstance(type) as IScenarioFactory;
                        factory.Assembly = Path.GetFileName(assembly.CodeBase);
                        factory.FactoryName = type.ToString();
                        if (String.IsNullOrEmpty(factoryName))
                            AddFactory(factory);
                        else if (Path.GetExtension(factory.FactoryName).TrimStart('.').Equals(factoryName,StringComparison.CurrentCultureIgnoreCase))
                            AddFactory(factory);
                    }
                }
            }

            if (countInitial == factories.Count && null != factoryName)
                throw new CouldNotFindScenarioFactoryException(factoryName);
        }

        private void AddFactory(IScenarioFactory factory)
        {
            // make sure we dont have the same factory twice.
            var found = factories.FirstOrDefault(p => p.FactoryName.Equals(factory.FactoryName, StringComparison.InvariantCultureIgnoreCase));

            if (null == found)
                factories.Add(factory);
        }
        
        ///<summary>
        ///This is a class that handles the singleton scenario instance.
        ///It is not exposed to anythigng because we only want once Scenario instance at a time.
        ///</summary>
        private class SingletonScenarioInstance
        {
            /// <summary>
            /// Scenario instance object
            /// </summary>
            internal static readonly ScenarioManager Instance = new ScenarioManager();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            /// </summary>
            static SingletonScenarioInstance()
            {
            }

            /// <summary>
            /// Basic Constructor.
            /// </summary>
            private SingletonScenarioInstance()
            {
            }
        }
    }
}
