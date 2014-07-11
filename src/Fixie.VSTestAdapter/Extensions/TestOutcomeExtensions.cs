namespace Fixie.VSTestAdapter.Extensions
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Results;

    public static class TestOutcomeExtensions
    {
        public static TestOutcome ToMsTestOutcome(this CaseStatus caseResultStatus)
        {
            switch (caseResultStatus)
            {
                case CaseStatus.Passed:
                    return TestOutcome.Passed;
                case CaseStatus.Failed:
                    return TestOutcome.Failed;
                case CaseStatus.Skipped:
                    return TestOutcome.Skipped;
                default:
                    return TestOutcome.None;
            }
        }
    }
}
