namespace Fixie.VSTestAdapter
{
    using System.IO;
    using System.Reflection;

    public class Utilities
    {
        internal static Assembly GetAssemblyFromPath(string source)
        {
            var assemblyFullPath = Path.GetFullPath(source);
            return Assembly.LoadFrom(assemblyFullPath);
        }

        internal static MethodInfo GetMethodInfo(Assembly assembly, string className, string displayName)
        {
            var type = assembly.GetType(className);
            var method = type.GetMethod(displayName);
            return method;
        }
    }
}
