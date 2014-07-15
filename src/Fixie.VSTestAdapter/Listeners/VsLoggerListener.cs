namespace Fixie.VSTestAdapter.Listeners
{
    using System;
    using System.Reflection;
    using System.Text;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using Results;

    [Serializable]
    public class VsLoggerListener : Listener
    {
        private readonly IMessageLogger logger;

        public VsLoggerListener(IMessageLogger logger)
        {
            this.logger = logger;
        }

        public void AssemblyStarted(Assembly assembly)
        {
            Message(string.Format("------ Testing Assembly {0} ------", assembly.FileName()));
        }

        public void CaseSkipped(SkipResult result)
        {
            Message(string.Format("Test '{0}' skipped{1}", result.Case.Name, result.Reason == null ? null : ": " + result.Reason));
        }

        public void CasePassed(PassResult result)
        {
        }

        public void CaseFailed(FailResult result)
        {
            Message(string.Format("Test '{0}' failed: {1}", result.Case.Name, result.ExceptionSummary.DisplayName));
            Message(result.ExceptionSummary.StackTrace);
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
            var assemblyName = typeof(VsLoggerListener).Assembly.GetName();
            var name = assemblyName.Name;
            var version = assemblyName.Version;

            var line = new StringBuilder();

            line.AppendFormat("{0} passed", result.Passed);
            line.AppendFormat(", {0} failed", result.Failed);

            if (result.Skipped > 0)
                line.AppendFormat(", {0} skipped", result.Skipped);

            line.AppendFormat(", took {0:N2} seconds", result.Duration.TotalSeconds);

            line.AppendFormat(" ({0} {1}).", name, version);
            Message(line.ToString());
        }

        void Message(string message, TestMessageLevel msgLevel = TestMessageLevel.Informational)
        {
            if (logger != null)
                logger.SendMessage(msgLevel, message);
        }

    }
}
