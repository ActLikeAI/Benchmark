using System;
using System.Diagnostics;
using System.Reflection;

namespace ActLikeAI.Benchmark
{
    internal class Benchmark
    {
        public string Name { get; }
        public string Class { get; }
        public string Method { get; }
        public int RunCount { get; }
        public bool WarmUp { get; }
        public bool ForceGC { get; }
        

        public Benchmark(MethodInfo method, Type type, BenchmarkAttribute attribute)
        {
            this.method = method;
            this.type = type;

            Class = $"{type.Name}";
            Method = $"{method.Name}";
            Name = !string.IsNullOrWhiteSpace(attribute.Name) ? attribute.Name : $"{Class}.{Method}";

            RunCount = attribute.RunCount;
            WarmUp = attribute.WarmUp;
            ForceGC = attribute.ForceGC;           
        }


        public Result Run()
        {
            var instance = Activator.CreateInstance(type);
            
            if (WarmUp)
                method.Invoke(instance, null);

            double[] measurements = new double[RunCount];
            DateTime time = DateTime.Now; 

            for (int i = 0; i < RunCount; i++)
            {
                if (ForceGC)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }

                var stopwatch = Stopwatch.StartNew();
                method.Invoke(instance, null);
                stopwatch.Stop();

                measurements[i] = stopwatch.Elapsed.TotalMilliseconds;
            }

            IDisposable? disposable = instance as IDisposable;
            if (disposable is not null)
                disposable.Dispose();

            //Calculating average runtime and standard deviation. If we have only one measurement, then 
            //the usual formula for standard deviation is not applicable, so we take it to be zero by definition.           
            double average = 0.0;
            double standardDeviation = 0.0;
            double min = double.MaxValue;
            double max = double.MinValue;

            if (measurements.Length == 1)
            {
                average = min = max = measurements[0];
            }
            else
            {
                for (int i = 0; i < measurements.Length; i++)
                {
                    average += measurements[i];

                    if (measurements[i] < min)
                        min = measurements[i];

                    if (measurements[i] > max)
                        max = measurements[i];
                }
                average /= measurements.Length;

                double variance = 0.0;
                for (int i = 0; i < measurements.Length; i++)
                    variance += (measurements[i] - average) * (measurements[i] - average);

                variance /= (measurements.Length - 1);
                standardDeviation = Math.Sqrt(variance);
            }

            return new Result("Current", Environment.MachineName, Class, Method, Name, average, standardDeviation, min, max, RunCount, time);
        }

        private readonly MethodInfo method;
        private readonly Type type;
    }
}
