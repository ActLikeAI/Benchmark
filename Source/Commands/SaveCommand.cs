using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace ActLikeAI.Benchmark
{
    internal class SaveCommand : Command<SaveCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
            [CommandArgument(0, "[set]")]
            [Description("Name under which current set is saved")]
            public string Set { get; set; } = "";
        }


        public override int Execute(
            [NotNull] CommandContext context,
            [NotNull] Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Set))
            {
                AnsiConsole.MarkupLine("Please provide a valid set name.");
                return 1;
            }

            string dataLocation = context.Data as string ?? "Benchmark";

            CsvResultFile pastFile = new(Path.Combine(dataLocation, "Results.csv"));
            var past = pastFile.Load();

            if (past is null)
                past = new();

            if (past.ContainsSet(settings.Set))
            {                
                bool overwrite = AnsiConsole.Confirm($"[aqua]{settings.Set}[/] exists in the results database. Do you want to overwrite?", false);

                if (overwrite)
                    past.RemoveSet(settings.Set);
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Exiting without save.[/]");
                    return 1;
                }
            }

            CsvResultFile currentFile = new(Path.Combine(dataLocation, "Current.csv"));
            var current = currentFile.Load();

            if (current is null)
            {
                AnsiConsole.MarkupLine("Can't find Current.csv. Please, run benchmarks at least once before saving.");
                return 1;
            }
           
            foreach (var result in current)
                past.Add(result with { Set = settings.Set });
            pastFile.Save(past);

            return 0;
        }
    }
}
