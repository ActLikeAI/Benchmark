using ActLikeAI.Benchmark;
using System.Threading;

namespace GettingStarted
{
    public class EccentricModel
    {
        [Benchmark(RunCount = 30)]
        public void LowEccentricity() 
            => Thread.Sleep(60);


        [Benchmark]
        public void Heartbeat() 
            => Thread.Sleep(80);


        //This won't be run unless benchmark runner is invoked with '-a' switch.
        [Benchmark(Skip = true)]
        public void LongCalculation()
            => Thread.Sleep(1000);


        //Static methods are ignored.
        [Benchmark]
        public static void StaticMethod() 
            => Thread.Sleep(100);


        //Private methods are ignored.
        [Benchmark]
        void PrivateMethod() 
            => Thread.Sleep(100);


        //Internal methods are ignored.
        [Benchmark]
        internal void InternalMethod() 
            => Thread.Sleep(100);
    }
}
