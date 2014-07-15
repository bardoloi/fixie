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
            var listener = new VsConsoleListener(frameworkHandle);
            var tests = TestDiscoverer.GetTests(sources, null, listener);

            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Starting RunTests");
            frameworkHandle.SendMessage(TestMessageLevel.Informational, Directory.GetCurrentDirectory());
            RunTests(tests, runContext, frameworkHandle);

            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Done with RunTests!");
            frameworkHandle.SendMessage(TestMessageLevel.Informational, Directory.GetCurrentDirectory());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testCases"></param>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        public void RunTests(IEnumerable<TestCase> testCases, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Starting RunTests 2");

            #region TODO: remove after testing.
            foreach (var testCase in testCases)
            {
                var res = new TestResult(testCase)
                {
                    Outcome = TestOutcome.Passed,
                    DisplayName = testCase.DisplayName                
                };
                frameworkHandle.RecordResult(res);
            }
            return;

            #endregion

            cancelled = false;

            var listener = new DummyListener();
            var runner = new Runner(listener);

            foreach (var testCase in testCases)
            {
                if (cancelled) break;

                var assembly = Utilities.GetAssemblyFromPath(testCase.Source);

                var methodInfo = testCase.LocalExtensionData as MethodInfo;
                  
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

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            cancelled = true;
        }
    }
}
