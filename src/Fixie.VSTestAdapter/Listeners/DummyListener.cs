namespace Fixie.VSTestAdapter.Listeners
{
    using System;
    using System.Reflection;
    using Results;

    [Serializable]
    public class DummyListener : Listener
    {
        public void AssemblyStarted(Assembly assembly)
        {
        }

        public void CaseSkipped(SkipResult result)
        {
        }

        public void CasePassed(PassResult result)
        {
        }

        public void CaseFailed(FailResult result)
        {
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
        }
    }
}
