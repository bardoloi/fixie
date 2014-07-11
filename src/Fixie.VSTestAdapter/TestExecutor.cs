namespace Fixie.VSTestAdapter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Listeners;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private bool cancelled;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var listener = new DummyListener();
            var runner = new Runner(listener);
            var tests = TestDiscoverer.GetTests(sources, null, runner);
            
            RunTests(tests, runContext, frameworkHandle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testCases"></param>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        public void RunTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            cancelled = false;

            var listener = new DummyListener();
            var runner = new Runner(listener);

            foreach (var testCase in testCases)
            {
                if (cancelled) break;

                var assembly = Utilities.GetAssemblyFromPath(testCase.Source);
                var methodInfo = Utilities.GetMethodInfo(   assembly,
                                                            testCase.CodeFilePath,  // this is a hack; we are getting the type info from this field
                                                            testCase.DisplayName);

                var runResult = runner.RunMethod(assembly, methodInfo);

                var testResult = new TestResult(testCase)
                {
                    Outcome = runResult.Failed > 0 ? TestOutcome.Failed : TestOutcome.Passed,
                    DisplayName = testCase.DisplayName,
                    Duration = runResult.Duration,                    
                };

                frameworkHandle.RecordResult(testResult);
            }
        }

        public void Cancel()
        {
            cancelled = true;
        }
    }
}
