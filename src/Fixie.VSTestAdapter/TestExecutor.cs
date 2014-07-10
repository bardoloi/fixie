namespace Fixie.VSTestAdapter
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Results;

    [FileExtension(".dll")]
    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        bool cancelled;

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var assemblyResults = new List<AssemblyResult>();
            var runner = new Runner(new DummyListener());

            // get the tests to run
            foreach (var source in sources)
            {
                var assembly = Assembly.LoadFrom(source);
                assemblyResults.Add(runner.RunAssembly(assembly));
            }

            // print the results
            foreach (var assemblyResult in assemblyResults)
            {
                foreach (var conventionResult in assemblyResult.ConventionResults)
                {
                    foreach (var classResult in conventionResult.ClassResults)
                    {
                        foreach (var caseResult in classResult.CaseResults)
                        {
                            // send the info back to the test results handler!
                            var testCase = new TestCase(caseResult.Name, Constants.ExecutorUri, assemblyResult.Name);
                            var testResult = new TestResult(testCase)
                            {
                                Duration = caseResult.Duration,
                                Outcome = caseResult.Status.ConvertToMsTestOutcome(),
                                ErrorMessage = (caseResult.ExceptionSummary != null) ? caseResult.ExceptionSummary.Message : string.Empty,
                                ErrorStackTrace = (caseResult.ExceptionSummary != null) ? caseResult.ExceptionSummary.StackTrace : string.Empty,
                            };

                            frameworkHandle.RecordResult(testResult);
                        }

                        // debug message
                        if (classResult.Failed > 0)
                        {
                            frameworkHandle.SendMessage(TestMessageLevel.Informational,
                                string.Format("Class: {0}, Passed: {1}, Failed: {2}, Skipped: {3}",
                                    classResult.Name,
                                    classResult.Passed,
                                    classResult.Failed,
                                    classResult.Skipped));
                        }
                    }
                }
            }

            // var tests = TestDiscoverer.GetTests(sources, null);
            // RunTests(tests, runContext, frameworkHandle);
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
                    Outcome = TestOutcome.Failed,
                    ErrorMessage = "Apples and bananas",
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
