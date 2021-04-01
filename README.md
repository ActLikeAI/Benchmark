[![NuGet](https://img.shields.io/badge/NuGet-0.2.0-321d4c)](https://www.nuget.org/packages/ActLikeAI.Benchmark/)
[![MIT license](https://img.shields.io/badge/License-MIT-blue)](https://github.com/ActLikeAI/Benchmark/blob/main/LICENSE)

# Introduction

ActLikeAI.Benchmark is a macro-benchmarking library for .Net 5. It's main goal is to provide an easy mechanism for monitoring long term performance of application's representative workloads. Collected measurements are saved in CSV files, so it's easy to plot them in the plotting library of choice.

# Getting started

Add new Console app to your solution and reference the latest version on [NuGet](https://www.nuget.org/packages/ActLikeAI.Benchmark/). Here is the minimal sample:

```C#
using ActLikeAI.Benchmark;

namespace SampleApp
{
    class Program
    {
        [Benchmark]
        public void Method() 
            => System.Threading.Thread.Sleep(100);

        static int Main(string[] args)
        {            
            Runner runner = new("Data");
            return runner.Invoke(args);
        }
    }
}

```
```Runner``` constructor takes a path to the directory in which results are saved (relative to the executable location). ```run``` command produces a CSV file with current measurements (```Current.csv```), while ```save``` command enters the current set into the results database (```Results.csv``` file) under a specified name. Each subsequent ```run``` displays the comparison between current and saved runs.

Note that methods marked with ```Bechmark``` attribute must be public non-static methods that take no parameters. See ```GettingStarted``` project in ```Samples``` directory for more complete sample.

