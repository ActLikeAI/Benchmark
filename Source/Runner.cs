using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Spectre.Console;
using Spectre.Console.Cli;

namespace ActLikeAI.Benchmark
{
    public class Runner
    {
        public Runner(string dataLocation)
        {
            this.dataLocation = dataLocation;
        }


        public int Invoke(string[] args)
        {
            var entryAssembly = Assembly.GetEntryAssembly();            
            if (entryAssembly is null)
            {
                AnsiConsole.MarkupLine("[red]Can't get entry assembly. Make sure that you are not calling Runner from unmanaged code.[/]");
                return 1;
            }

            var attributes = entryAssembly.GetCustomAttributes<DebuggableAttribute>();
            foreach (var attribute in attributes)
                if (attribute.IsJITOptimizerDisabled)
                {
                    AnsiConsole.MarkupLine("[red]Assembly was compiled in Debug mode. Use Release mode to get reliable benchmarks.[/]");
                    return 2;
                }
           
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.SetApplicationName(Path.GetFileNameWithoutExtension(entryAssembly.Location) ?? "ActLikeAI.Benchmark");
                config.CaseSensitivity(CaseSensitivity.None);
                config.AddCommand<ListCommand>("list")
                    .WithDescription("Lists all benchmarks and exits");
                config.AddCommand<RunCommand>("run")
                    .WithData(dataLocation)
                    .WithDescription("Runs the benchmarks");
                config.AddCommand<SaveCommand>("save")
                    .WithData(dataLocation)
                    .WithDescription("Saves last run to benchmark collection");
            });

            int result;
            try
            {
                result = app.Run(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                result = 1;
            }

            return result;                
        }

        private readonly string dataLocation;
    }
}
