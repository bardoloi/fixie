namespace Fixie.VSTestAdapter.Extensions
{
    using System.Reflection;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    
    public static class CaseConversion
    {
        /// <summary>
        /// Convert a Fixie Case instance into an MS TestCase instance
        /// </summary>
        /// <param name="method"></param>
        /// <param name="assemblyFullPath"></param>
        /// <returns></returns>
        public static TestCase AsMSTestCase(this MethodInfo method, string assemblyFullPath)
        {
            var fullyQualifiedName = method.DeclaringType.FullName + '.' + method.Name;
            var executorUri = VSTestAdapter.Constants.ExecutorUri;

            return new TestCase(fullyQualifiedName, executorUri, assemblyFullPath)
            {
                CodeFilePath = method.DeclaringType.Assembly.CodeBase, 
                DisplayName = method.Name,
                // this is a hack; since we need to send this info back to the runner the MS TestCase object
                LocalExtensionData = method 
            };
        }
    }
}
