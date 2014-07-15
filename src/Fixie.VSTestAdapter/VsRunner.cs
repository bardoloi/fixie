namespace Fixie
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Results;

    public class VsRunner : MarshalByRefObject
    {
        public AssemblyResult RunAssembly(string assemblyFullPath, Listener listener)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));

            var runner = new Runner(listener);
            return runner.RunAssembly(assembly);
        }

        public AssemblyResult RunMethod(string assemblyFullPath, Listener listener, MethodInfo method)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));

            var runner = new Runner(listener);
            return runner.RunMethod(assembly, method);            
        }

        public IEnumerable<MethodInfo> GetTestMethods(string assemblyFullPath, Listener listener)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));
            var runner = new Runner(listener);
            return runner.GetTestMethodsInAssembly(assembly);
        }

    }
}


