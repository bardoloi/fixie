namespace Fixie.VSTestAdapter
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

    [DefaultExecutorUri(Constants.ExecutorUriString)]
    public class TestDiscoverer : ITestDiscoverer
    {
        /// <summary>
        /// Given the list of test sources, pulls out the test cases to be run from them
        /// </summary>
        /// <param name="sources">Sources that are passed in from the client (VS, command line etc.)</param>
        /// <param name="discoveryContext">Context/runsettings for the current test run.</param>
        /// <param name="logger">Used to relay warnings and error messages to the registered loggers.</param>
        /// <param name="discoverySink">Used to send back the test cases to the framework as and when discovered. Also responsible for discovery related events.</param>
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            if (discoverySink == null) return;

            var testCases = GetTests(sources, discoverySink);
            testCases.ToList().ForEach(discoverySink.SendTestCase);
        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var discoveredTests = new TestCase[3];

            discoveredTests[0] = new TestCase("Test Case 1", Constants.ExecutorUri, sources.FirstOrDefault());
            discoveredTests[1] = new TestCase("Test Case 2", Constants.ExecutorUri, sources.FirstOrDefault());
            discoveredTests[2] = new TestCase("Test Case 3", Constants.ExecutorUri, sources.FirstOrDefault());
            
            if (discoverySink != null)
            {
                foreach (var test in discoveredTests)
                {
                    discoverySink.SendTestCase(test);
                }
            }

            return discoveredTests;
        }
    }
}
