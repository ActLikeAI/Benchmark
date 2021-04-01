using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ActLikeAI.Benchmark
{
    internal class BenchmarkCollection : IEnumerable<Benchmark>
    {
        public int Count 
            => benchmarks.Count;

        public Benchmark this[int index] 
            => benchmarks[index];


        public BenchmarkCollection(Assembly assembly, bool all = false)
        {
            benchmarks = new();
            Type[] classes = assembly.GetExportedTypes();

            foreach (var benchmarkClass in classes)
            {
                var methods = benchmarkClass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    BenchmarkAttribute? attribute = method.GetCustomAttribute<BenchmarkAttribute>();

                    if (attribute is not null && method.GetParameters().Length == 0)
                        if (all || !attribute.Skip)
                            benchmarks.Add(new Benchmark(method, benchmarkClass, attribute));
                }
            }
        }


        public IEnumerator<Benchmark> GetEnumerator() 
            => ((IEnumerable<Benchmark>)benchmarks).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable)benchmarks).GetEnumerator();

        private readonly List<Benchmark> benchmarks;
    }
}
