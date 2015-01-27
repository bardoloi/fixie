﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Fixie.Execution;

namespace Fixie.ConsoleRunner
{
    public class TeamCityListener : MarshalByRefObject, Listener
    {
        public void AssemblyStarted(AssemblyInfo assembly)
        {
            Message("testSuiteStarted name='{0}'", SuiteName(assembly));
        }

        public void CaseSkipped(SkipResult result)
        {
            Message("testIgnored name='{0}'", result.Name);
        }

        public void CasePassed(PassResult result)
        {
            Message("testStarted name='{0}'", result.Name);
            Output(result.Name, result.Output);
            Message("testFinished name='{0}' duration='{1}'", result.Name, DurationInMilliseconds(result.Duration));
        }

        public void CaseFailed(FailResult result)
        {
            Message("testStarted name='{0}'", result.Name);
            Output(result.Name, result.Output);
            Message("testFailed name='{0}' message='{1}' details='{2}'", result.Name, result.Exceptions.PrimaryException.Message, result.Exceptions.CompoundStackTrace);
            Message("testFinished name='{0}' duration='{1}'", result.Name, DurationInMilliseconds(result.Duration));
        }

        public void AssemblyCompleted(AssemblyInfo assembly, AssemblyResult result)
        {
            Message("testSuiteFinished name='{0}'", SuiteName(assembly));
        }

        static void Message(string format, params string[] args)
        {
            var encodedArgs = args.Select(Encode).Cast<object>().ToArray();
            Console.WriteLine("##teamcity["+format+"]", encodedArgs);
        }

        static void Output(string name, string output)
        {
            if (!String.IsNullOrEmpty(output))
                Message("testStdOut name='{0}' out='{1}'", name, output);
        }

        static string Encode(string value)
        {
            var builder = new StringBuilder();
            
            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '|': builder.Append("||"); break;
                    case '\'': builder.Append("|'"); break;
                    case '[': builder.Append("|["); break;
                    case ']': builder.Append("|]"); break;
                    case '\n': builder.Append("|n"); break; // Line Feed
                    case '\r': builder.Append("|r"); break; // Carriage Return
                    case '\u0085': builder.Append("|x"); break;  // Next Line
                    case '\u2028': builder.Append("|l"); break;  // Line Separator
                    case '\u2029': builder.Append("|p"); break;  // Paragraph Separator
                    default: builder.Append(ch); break;
                }
            }

            return builder.ToString();
        }

        static string DurationInMilliseconds(TimeSpan duration)
        {
            return ((int)Math.Ceiling(duration.TotalMilliseconds)).ToString();
        }

        static string SuiteName(AssemblyInfo assembly)
        {
            return Path.GetFileName(assembly.Location);
        }

        public override object InitializeLifetimeService()
        {
            return null; //Allowing the instance to live indefinitely.
        }
    }
}