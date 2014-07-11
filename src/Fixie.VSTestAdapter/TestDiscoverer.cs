namespace Fixie.VSTestAdapter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Listeners;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [FileExtension(".dll")]
    [DefaultExecutorUri(Constants.ExecutorUriString)]
    public class TestDiscoverer : ITestDiscoverer
    {
        /// <summary>
        /// Given a list of test sources, pulls out the test cases to be run from them
        /// </summary>
        /// <param name="sources">Sources that are passed in from the client (VS, command line etc.)</param>
        /// <param name="discoveryContext">Context/runsettings for the current test run.</param>
        /// <param name="logger">Used to relay warnings and error messages to the registered loggers.</param>
        /// <param name="discoverySink">Used to send back the test cases to the framework as and when discovered. Also responsible for discovery related events.</param>
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            if (discoverySink != null)
            {
                var listener = new DummyListener();
                var runner = new Runner(listener);

                var testCases = GetTests(sources, discoverySink, runner);
                foreach (var testCase in testCases)
                {
                    discoverySink.SendTestCase(testCase);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="discoverySink"></param>
        /// <param name="runner"></param>
        /// <returns></returns>
        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink, Runner runner)
        {
            var discoveredTestCases = new List<TestCase>();

            foreach (var source in sources)
            {
                var assembly = Utilities.GetAssemblyFromPath(source);

                // using local variable to prevent "accessed foreach variable in a closure" warning below
                var mySource = source;  
                
                var casesInAssembly = runner.GetCasesInAssembly(assembly)
                                            .Select(@case => @case.ToMsTestCase(mySource))
                                            .Distinct();

                discoveredTestCases.AddRange(casesInAssembly);
            }
            
            if (discoverySink != null)
            {
                foreach (var testCase in discoveredTestCases)
                {
                    discoverySink.SendTestCase(testCase);
                }
            }

            return discoveredTestCases.AsEnumerable();
        }
    }
}
