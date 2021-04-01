using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ActLikeAI.Benchmark
{
    internal abstract class BenchmarkCommand<T> : Command<T> where T : CommandSettings
    {
        protected static string GetBenchmarkVersion()
        {
            var library = Assembly.GetExecutingAssembly();
            if (library is null)            
                throw new ArgumentException("Can't get assembly. Make sure that you are not calling Benchmark library from unmanaged code.");

            var version = library.GetName().Version;
            if (version is null)
                throw new ArgumentException("Can't determine ActLikeAI.Benchmark's version. Aborting.");

            return $"{version.Major}.{version.Minor}.{version.Build}";
        }


        protected static BenchmarkCollection GetBenchmarks(bool all)
        {
            var exe = Assembly.GetEntryAssembly();
            if (exe is null)            
                throw new ArgumentException("Can't get assembly. Make sure that you are not calling Benchmark library from unmanaged code.");                
            
            return new BenchmarkCollection(exe, all);
        }
    }
}
