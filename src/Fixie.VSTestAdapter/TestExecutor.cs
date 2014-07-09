namespace Fixie.VSTestAdapter
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private bool cancelled;

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var tests = TestDiscoverer.GetTests(sources, null);

            RunTests(tests, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            cancelled = false;

            foreach (var test in tests)
            {
                if (cancelled)
                {
                    break;
                }

                // Setup the test result as indicated by the test case.
                var testResult = new TestResult(test)
                {
                    Outcome = (TestOutcome)test.GetPropertyValue(TestResultProperties.Outcome),
                    ErrorMessage = (string)test.GetPropertyValue(TestResultProperties.ErrorMessage),
                    ErrorStackTrace = (string)test.GetPropertyValue(TestResultProperties.ErrorStackTrace)
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
