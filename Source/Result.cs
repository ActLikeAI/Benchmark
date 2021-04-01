using System;

namespace ActLikeAI.Benchmark
{
    internal record Result(       
        string Set,
        string Computer,
        string Class,
        string Method,
        string Name,
        double AverageRuntime,
        double StandardDeviation,
        double MinRuntime,
        double MaxRuntime,
        int RunCount,
        DateTime Time);
}
