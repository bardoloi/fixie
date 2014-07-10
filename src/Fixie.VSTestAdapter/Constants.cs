namespace Fixie.VSTestAdapter
{
    using System;

    public static class Constants
    {
        public const string ExecutorUriString = @"executor://fixie.VSTestRunner/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
