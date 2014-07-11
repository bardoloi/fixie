namespace Fixie.VSTestAdapter.Listeners
{
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Results;

    public class VsClientListener : Listener
    {
        private readonly IFrameworkHandle frameworkHandle;

        public VsClientListener(IFrameworkHandle frameworkHandle)
        {
            this.frameworkHandle = frameworkHandle;
        }

        public void AssemblyStarted(Assembly assembly)
        {
            SendMessage(string.Format("------ Testing Assembly {0} ------", assembly.FileName()));
        }

        public void CaseSkipped(SkipResult result)
        {
            SendMessage(string.Format("Test '{0}' skipped{1}", result.Case.Name, result.Reason == null ? null : ": " + result.Reason));
        }

        public void CasePassed(PassResult result)
        {
        }

        public void CaseFailed(FailResult result)
        {
            SendMessage(string.Format("Test '{0}' failed: {1}", result.Case.Name, result.ExceptionSummary.DisplayName));
            SendMessage(result.ExceptionSummary.StackTrace);
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
            var assemblyName = typeof(VsClientListener).Assembly.GetName();
            var name = assemblyName.Name;
            var version = assemblyName.Version;

            var line = new StringBuilder();

            line.AppendFormat("{0} passed", result.Passed);
            line.AppendFormat(", {0} failed", result.Failed);

            if (result.Skipped > 0)
                line.AppendFormat(", {0} skipped", result.Skipped);

            line.AppendFormat(", took {0:N2} seconds", result.Duration.TotalSeconds);

            line.AppendFormat(" ({0} {1}).", name, version);
            SendMessage(line.ToString());
        }

        void SendMessage(string message, TestMessageLevel msgLevel = TestMessageLevel.Informational)
        {
            if (frameworkHandle != null)
                frameworkHandle.SendMessage(msgLevel, message);
        }
    }
}
