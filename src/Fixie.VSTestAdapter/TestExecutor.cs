namespace Fixie.VSTestAdapter
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Results;

    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        bool cancelled;

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var results = new List<AssemblyResult>();
            var runner = new Runner(new DummyListener());

            foreach (var source in sources)
            {
                try
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "======= Now in source " + source);
                    var assembly = Assembly.LoadFrom(source);
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Going into the runner now.");
                    results.Add(runner.RunAssembly(assembly));
                }
                catch (Exception ex)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Oops! I broke. Message: " + ex.Message);
                }
            }

            foreach (var result in results)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, 
                                            string.Format("Name: {0}, Total: {1}, Failed: {2}", 
                                            result.Name, 
                                            result.Total, 
                                            result.Failed));
                foreach (var conventionResult in result.ConventionResults)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "--- convention results");
                    frameworkHandle.SendMessage(TestMessageLevel.Informational,
                                                string.Format("Name: {0}, Passed: {1}, Failed: {2}, Skipped: {3}", 
                                                conventionResult.Name, 
                                                conventionResult.Passed, 
                                                conventionResult.Failed, 
                                                conventionResult.Skipped));
                    foreach (var classResult in conventionResult.ClassResults)
                    {
                        frameworkHandle.SendMessage(TestMessageLevel.Informational, "--- class results");
                        frameworkHandle.SendMessage(TestMessageLevel.Informational,
                                                    string.Format("Name: {0}, Passed: {1}, Failed: {2}, Skipped: {3}", 
                                                    classResult.Name, 
                                                    classResult.Passed, 
                                                    classResult.Failed, 
                                                    classResult.Skipped));
                        foreach (var caseResult in classResult.CaseResults)
                        {
                            frameworkHandle.SendMessage(TestMessageLevel.Informational, "--- CASE results");
                            frameworkHandle.SendMessage(TestMessageLevel.Informational,
                                                        string.Format("Name: {0}, Status: {1}, Exception: {2}", 
                                                        caseResult.Name,
                                                        caseResult.Status.ToDisplayString(),
                                                        caseResult.ExceptionSummary.ToDisplayString()));                            
                        }                        
                    }
                    
                }
            }

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
