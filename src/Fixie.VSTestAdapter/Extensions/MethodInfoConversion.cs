namespace Fixie.VSTestAdapter.Extensions
{
    using System.Reflection;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    public static class MethodInfoConversion
    {
        public static TestCase AsMSTestCase(this MethodInfo method, string source)
        {
            return new TestCase( "The time is now!", //method.GetType().FullName + '.' + method.Name,
                                VSTestAdapter.Constants.ExecutorUri, 
                                source);
        }
    }
}
