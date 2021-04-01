using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Spectre.Console;
using Spectre.Console.Cli;

namespace ActLikeAI.Benchmark
{
    internal class ListCommand : BenchmarkCommand<ListCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
            [CommandOption("-a|--all")]
            [Description("List all benchmarks.")]
            public bool ListAll { get; set; }
        }


        public override int Execute(
            [NotNull] CommandContext context, 
            [NotNull] Settings settings)
        {
            var version = GetBenchmarkVersion();
            var benchmarks = GetBenchmarks(settings.ListAll);
            var defaults = new BenchmarkAttribute();

            AnsiConsole.WriteLine($"ActLikeAI.Benchmark {version} found {benchmarks.Count} benchmark{(benchmarks.Count == 1 ? "" : "s")}.");            
            AnsiConsole.WriteLine();

            var table = new Table();                                     
            table.AddColumn("Benchmark", column => column.LeftAligned());
            table.AddColumn("Class", column => column.LeftAligned());
            table.AddColumn("Method", column => column.LeftAligned());            
            table.AddColumn("#", column => column.RightAligned());
            table.AddColumn("Warm-Up", column => column.Centered());
            table.AddColumn("Force GC", column => column.Centered());
         
            foreach (var benchmark in benchmarks)
            {
                table.AddRow(
                    $"[aqua]{benchmark.Name}[/]", 
                    benchmark.Class, 
                    benchmark.Method, 
                    $"[{(benchmark.RunCount == defaults.RunCount ? "grey" : "yellow")}]{benchmark.RunCount}[/]",
                    $"[{(benchmark.WarmUp == defaults.WarmUp ? "grey" : "yellow")}]{benchmark.WarmUp}[/]",
                    $"[{(benchmark.ForceGC == defaults.ForceGC ? "grey" : "yellow")}]{benchmark.ForceGC}[/]");
            }

            AnsiConsole.Render(table);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]DarkGray[/] - default value of the property");
            AnsiConsole.WriteLine();

            return 0;
        }
    }
}
