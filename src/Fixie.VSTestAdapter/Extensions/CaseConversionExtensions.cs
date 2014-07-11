namespace Fixie.VSTestAdapter.Extensions
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    public static class CaseConversionExtensions
    {
        /// <summary>
        /// Convert a Fixie Case instance into an MS TestCase instance
        /// </summary>
        /// <param name="case"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TestCase ToMsTestCase(this Case @case, string source)
        {
            return new TestCase(@case.Name, VSTestAdapter.Constants.ExecutorUri, source)
            {
                CodeFilePath = @case.Class.FullName, // this is a hack; since we need to send this info via the MS TestCase object
                DisplayName = @case.Name
            };
        }
    }
}
