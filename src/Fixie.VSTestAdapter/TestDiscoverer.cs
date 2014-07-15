namespace Fixie.VSTestAdapter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
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
                logger.SendMessage(TestMessageLevel.Informational, "Starting getTests()");
                var listener = new VsLoggerListener(logger);
                GetTests(sources, discoverySink, listener);
                logger.SendMessage(TestMessageLevel.Informational, "Ending getTests()");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="discoverySink"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink, Listener listener)
        {
            var discoveredTestCases = new List<TestCase>();

            #region Quick Test to verify discoverer is working

             // foreach (var source in sources)
             // {
             //     discoveredTestCases.Add(new TestCase(source + ": The time is now: " + DateTime.Now.ToString("G"), Constants.ExecutorUri, source));
             //     Thread.Sleep(300);

             //     discoveredTestCases.Add(new TestCase(source + ": The time is now: " + DateTime.Now.ToString("G"), Constants.ExecutorUri, source));
             //     Thread.Sleep(300);

             //     discoveredTestCases.Add(new TestCase(source + ": The time is now: " + DateTime.Now.ToString("G"), Constants.ExecutorUri, source));
             // }
            #endregion

           try
           {
               foreach (var source in sources)
               {
                   using (var environment = new ExecutionEnvironment(source))
                   {
                       var runner = environment.Create<VsRunner>();
                       var fixieTests = runner.GetTestMethods(source, listener);
                       // var msTests = fixieTests.Select(method => method.AsMSTestCase(source));
                       // discoveredTestCases.AddRange(msTests);
                       discoveredTestCases.Add(new TestCase(source + ": The time is now: " + DateTime.Now.ToString("G"), Constants.ExecutorUri,
                           source));
                       Thread.Sleep(300);
                   }
               }
           }
           catch (ReflectionTypeLoadException ex)
           {
               var sb = new StringBuilder();
               foreach (var exSub in ex.LoaderExceptions)
               {
                   sb.AppendLine(exSub.Message);
                   var exFileNotFound = exSub as FileNotFoundException;
                   if (exFileNotFound != null)
                   {
                       if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                       {
                           sb.AppendLine("Fusion Log:");
                           sb.AppendLine(exFileNotFound.FusionLog);
                       }
                   }
                   sb.AppendLine();
               }
               var errorMessage = sb.ToString();

               discoveredTestCases.Add(new TestCase(errorMessage, Constants.ExecutorUri, sources.First()));
           }
           catch (TargetInvocationException ex)
           {
               discoveredTestCases.Add(new TestCase(ex.InnerException.ToString(), Constants.ExecutorUri, sources.First()));   
           }            
           catch (Exception ex)
           {
               discoveredTestCases.Add(new TestCase(ex.Message, Constants.ExecutorUri, sources.First()));   
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
