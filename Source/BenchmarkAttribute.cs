using System;

namespace ActLikeAI.Benchmark
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BenchmarkAttribute : Attribute
    {
        public string Name { get; set; } = "";
        public int RunCount { get; set; } = 10;        
        public bool WarmUp { get; set; } = true;
        public bool ForceGC { get; set; } = true;
        public bool Skip { get; set; } = false;        
    }
}
