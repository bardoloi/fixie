namespace Fixie
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Listeners;
    using Results;

    public class VsRunner : MarshalByRefObject
    {
        private DummyListener listener;

        public VsRunner()
        {
            this.listener = new DummyListener();
        }

        public AssemblyResult RunAssembly(string assemblyFullPath)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));

            var runner = new Runner(listener);
            return runner.RunAssembly(assembly);
        }

        public AssemblyResult RunMethod(string assemblyFullPath, MethodInfo method)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));

            var runner = new Runner(listener);
            return runner.RunMethod(assembly, method);            
        }

        public IEnumerable<MethodInfo> GetTestMethods(string assemblyFullPath)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));
            var runner = new Runner(listener);
            return runner.GetTestMethodsInAssembly(assembly);
        }

    }
}


