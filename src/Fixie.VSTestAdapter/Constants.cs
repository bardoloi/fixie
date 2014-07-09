namespace Fixie.VSTestAdapter
{
    using System;

    public static class Constants
    {
        public const string ExecutorUriString = @"executor://fixieVSTestRunner";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
