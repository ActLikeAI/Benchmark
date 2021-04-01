using ActLikeAI.Benchmark;

namespace GettingStarted
{
    class Program
    {
        static int Main(string[] args)
        {
           /* Compile the executable in Release mode. Run 'GettingStarted list'
            * to get a list of benchmarks along with their parameters. There 
            * should be seven of them.
            * Type 'GettingStarted run' to execute the benchmarks. This should take a while.
            * Now save the current run with 'GettingStarted save Test01'. 
            * Run 'GettingStarted run' again to see the comparison between current ans 
            * saved run. Baseline for the relative column can be changed via
            * '--baseline name' switch. Current is the default.
            */

            var runner = new Runner("Data");
            return runner.Invoke(args);
        }
    }
}
