using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ActLikeAI.Benchmark
{
    internal class RunCommand : BenchmarkCommand<RunCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
            [CommandOption("-a|--all")]
            [Description("Run all benchmarks.")]
            public bool RunAll { get; set; }

            [CommandOption("-c|--compact")]
            [Description("Display current run in compact form.")]
            public bool Compact { get; set; }

            [CommandOption("-b|--baseline [baseline]")]
            [Description("Baseline for comparison")]
            public FlagValue<string> Baseline { get; set; } = new();
        }


        public override int Execute(
            [NotNull] CommandContext context,
            [NotNull] Settings settings)
        {
            var results = RunBechmarks(settings.RunAll);
            string dataLocation = context.Data as string ?? "Benchmark";

            if (!Directory.Exists(dataLocation))
                Directory.CreateDirectory(dataLocation);

            CsvResultFile csv = new(Path.Combine(dataLocation, "Current.csv"));
            csv.Save(results);

            if (settings.Compact)
                DisplayCompact(results);
            else
            {                
                DisplayFull(results, dataLocation, settings.Baseline.IsSet ? settings.Baseline.Value : "Current");
            }
            return 0;
        }


        static ResultCollection RunBechmarks(bool all)
        {
            var version = GetBenchmarkVersion();
            var benchmarks = GetBenchmarks(all);

            AnsiConsole.WriteLine($"ActLikeAI.Benchmark {version} found {benchmarks.Count} benchmark{(benchmarks.Count == 1 ? "" : "s")}.");

            var results = new ResultCollection(benchmarks.Count);
            AnsiConsole.Progress()
                .AutoRefresh(false)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn()
                    {
                        FinishedStyle = new Style(Color.Lime)
                    },
                    new PercentageColumn()
                    {
                        CompletedStyle = Style.Plain
                    }
                })
                .Start(ctx =>
                {
                    // Define tasks
                    var task = ctx.AddTask("Running");
                    ctx.Refresh();

                    for (int i = 0; i < benchmarks.Count; i++)
                    {
                        var result = benchmarks[i].Run();
                        results.Add(result);
                        task.Increment(100.0 / benchmarks.Count);
                        ctx.Refresh();
                    }
                    task.Increment(10.0); //Just in case that we don't get to 100% due to the rounding errors.
                    ctx.Refresh();
                });

            return results;
        }


        static void DisplayCompact(ResultCollection results)
        {
            var table = new Table();
            table.AddColumn("Benchmark", column => column.LeftAligned());
            table.AddColumn("Avg [[ms]]", column => column.Centered());
            table.AddColumn("SD [[ms]]", column => column.Centered());
            table.AddColumn("Min [[ms]]", column => column.Centered());
            table.AddColumn("Max [[ms]]", column => column.Centered());
            table.AddColumn("#", column => column.RightAligned());

            foreach (var result in results)
            {
                table.AddRow(
                    $"[aqua]{result.Name}[/]",
                    $"{result.AverageRuntime:0.000}",
                    $"{result.StandardDeviation:0.000}",
                    $"{result.MinRuntime:0.000}",
                    $"{result.MaxRuntime:0.000}",
                    $"{result.RunCount}");
            }

            AnsiConsole.Render(table);
            AnsiConsole.WriteLine();
        }


        static void DisplayFull(ResultCollection currentResults, string dataLocation, string baseline)
        {
            CsvResultFile pastFile = new(Path.Combine(dataLocation, "Results.csv"));
            var pastResults = pastFile.Load();

            if (pastResults is null)
                DisplayCompact(currentResults);
            else
            {
                bool anyBaseFound = false;
                foreach (var current in currentResults)
                {
                    var past = pastResults
                        .Where(res => 
                            res.Computer == Environment.MachineName &&
                            res.Class == current.Class &&
                            res.Method == current.Method)
                        .OrderBy(res => res.Time);

                    bool baseFound = false;
                    double baseValue = double.NaN;
                    if (baseline.Equals("Current", StringComparison.InvariantCultureIgnoreCase))
                    {
                        anyBaseFound = true;
                        baseFound = true;
                        baseValue = current.AverageRuntime;
                        
                    }
                    else
                        foreach (var result in past)                       
                            if (baseline.Equals(result.Set, StringComparison.InvariantCultureIgnoreCase))
                            {
                                anyBaseFound = true;
                                baseFound = true;
                                baseValue = result.AverageRuntime;
                            }

                    AnsiConsole.MarkupLine($"Benchmark [fuchsia]{current.Name}[/]:");
                    AnsiConsole.WriteLine();

                    var table = new Table();
                    table.AddColumn("Set", column => column.LeftAligned());
                    table.AddColumn("Relative", column => column.Centered());
                    table.AddColumn("Avg [[ms]]", column => column.Centered());
                    table.AddColumn("SD [[ms]]", column => column.Centered());
                    table.AddColumn("Min [[ms]]", column => column.Centered());
                    table.AddColumn("Max [[ms]]", column => column.Centered());
                    table.AddColumn("#", column => column.RightAligned());
                    table.AddColumn("Time", column => column.Centered());


                    string relative = "NA";
                    string relativeColor = "grey";

                    foreach (var result in past)
                    {                        
                        if (baseFound)
                        {
                            relative = $"{result.AverageRuntime / baseValue: 0.000}";
                            if (baseline.Equals(result.Set, StringComparison.InvariantCultureIgnoreCase))
                                relativeColor = "yellow";
                            else
                                relativeColor = result.AverageRuntime < baseValue ? "lime" : "red";
                        }

                        table.AddRow(
                            $"[aqua]{result.Set}[/]",
                            $"[{relativeColor}]{relative:0.000}[/]",
                            $"{result.AverageRuntime:0.000}",
                            $"{result.StandardDeviation:0.000}",
                            $"{result.MinRuntime:0.000}",
                            $"{result.MaxRuntime:0.000}",
                            $"{result.RunCount}",
                            $"{result.Time:yyyy-MM-dd HH:mm:ss}");
                    }

                    if (baseFound)
                    {
                        relative = $"{current.AverageRuntime / baseValue: 0.000}";
                        if (baseline.Equals(current.Set, StringComparison.InvariantCultureIgnoreCase))
                            relativeColor = "yellow";
                        else
                            relativeColor = current.AverageRuntime < baseValue ? "lime" : "red";
                    }

                    table.AddRow(
                        $"[aqua]{current.Set}[/]",
                        $"[{relativeColor}]{relative:0.000}[/]",
                        $"{current.AverageRuntime:0.000}",
                        $"{current.StandardDeviation:0.000}",
                        $"{current.MinRuntime:0.000}",
                        $"{current.MaxRuntime:0.000}",
                        $"{current.RunCount}",
                        $"{current.Time:yyyy-MM-dd HH:mm:ss}");

                    AnsiConsole.Render(table);
                    AnsiConsole.WriteLine();
                }

                if (anyBaseFound)
                {
                    AnsiConsole.WriteLine("Legend:");
                    AnsiConsole.MarkupLine("    [yellow]Yellow[/] - baseline");
                    AnsiConsole.MarkupLine("    [lime]Green [/] - faster than baseline");
                    AnsiConsole.MarkupLine("    [red]Red   [/] - slower than baseline");
                    AnsiConsole.WriteLine();
                }
            }
        }
    }
}
