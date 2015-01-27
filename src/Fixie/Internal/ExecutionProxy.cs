﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Fixie.Execution;

namespace Fixie.Internal
{
    public class ExecutionProxy : MarshalByRefObject
    {
        public IReadOnlyList<MethodGroup> DiscoverTestMethodGroups(string assemblyFullPath, Options options)
        {
            var assembly = LoadAssembly(assemblyFullPath);

            return new Discoverer(options).DiscoverTestMethodGroups(assembly);
        }

        public AssemblyResult RunAssembly(string assemblyFullPath, Options options, Listener listener)
        {
            var assembly = LoadAssembly(assemblyFullPath);

            return Runner(options, listener).RunAssembly(assembly);
        }

        public AssemblyResult RunMethods(string assemblyFullPath, Options options, Listener listener, MethodGroup[] methodGroups)
        {
            var assembly = LoadAssembly(assemblyFullPath);

            return Runner(options, listener).RunMethods(assembly, methodGroups);
        }

        static Assembly LoadAssembly(string assemblyFullPath)
        {
            return Assembly.Load(AssemblyName.GetAssemblyName(assemblyFullPath));
        }

        static Runner Runner(Options options, Listener listener)
        {
            return new Runner(listener, options);
        }

        public override object InitializeLifetimeService()
        {
            return null; //Allowing the instance to live indefinitely.
        }
    }
}