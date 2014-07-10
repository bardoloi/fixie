namespace Fixie.VSTestAdapter
{
    using System;

    public static class Constants
    {
        public const string ExecutorUriString = @"executor://fixie.VSTestRunner";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
