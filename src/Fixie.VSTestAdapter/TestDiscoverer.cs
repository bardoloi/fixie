namespace Fixie.VSTestAdapter
{
    using System.Collections.Generic;
    using System.Linq;
    using Conventions;
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
            var testCases = GetTests(sources, discoverySink);
            if (discoverySink != null)
            {
                testCases.ToList().ForEach(discoverySink.SendTestCase);                
            }
        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var testCases = new List<TestCase>();

            // TODO: call Fixie's test discoverer

            var discoveredTests = new TestCase[3];

            discoveredTests[0] = new TestCase("Test Case 1", VSTestAdapter.Constants.ExecutorUri, "RandomSource1");
            discoveredTests[1] = new TestCase("Test Case 2", VSTestAdapter.Constants.ExecutorUri, "RandomSource2");
            discoveredTests[2] = new TestCase("Test Case 3", VSTestAdapter.Constants.ExecutorUri, "RandomSource3");

            discoveredTests[0].SetPropertyValue(TestResultProperties.Outcome, TestOutcome.Failed);
            discoveredTests[1].SetPropertyValue(TestResultProperties.Outcome, TestOutcome.Passed);
            discoveredTests[2].SetPropertyValue(TestResultProperties.Outcome, TestOutcome.Passed);

            discoveredTests[0].SetPropertyValue(TestResultProperties.ErrorMessage, "Apples and bananas");
            discoveredTests[1].SetPropertyValue(TestResultProperties.ErrorMessage, "Eeples and baneenees");
            discoveredTests[2].SetPropertyValue(TestResultProperties.ErrorMessage, "Ooples and banoonoons");
            
           // foreach (var source in sources)
           // {
           //     var discoveredTests = GetFixieDiscoveredTests(source);
            // }
            foreach (var test in discoveredTests)
            {
                if (discoverySink != null)
                {
                    discoverySink.SendTestCase(test);
                }
                testCases.Add(test);
            }

            return testCases;
        }

        internal static IEnumerable<TestCase> DiscoverFixieTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var testCases = new List<TestCase>();

            var config = new ConfigModel();
            var discoveryContext = new DiscoveryModel(config);

            foreach (var source in sources)
            {
                var testCase = discoveryContext.TestMethods(source.GetType());
                


            }


            foreach (var test in discoveredTests)
            {
                if (discoverySink != null)
                {
                    discoverySink.SendTestCase(test);
                }
                testCases.Add(test);
            }

            return testCases;
        }


    }
}
