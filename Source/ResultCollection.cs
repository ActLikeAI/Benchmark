using System;
using System.Collections;
using System.Collections.Generic;

namespace ActLikeAI.Benchmark
{
    internal class ResultCollection : IEnumerable<Result>
    {       
        public int Count 
            => results.Count;

        public void Add(Result item) 
            => results.Add(item);
        
        public void AddRange(IEnumerable<Result> items) 
            => results.AddRange(items);

        public bool ContainsSet(string set)
        {
            bool found = false;
            foreach (var result in results)
                if (result.Set == set)
                {
                    found = true;
                    break;
                }

            return found;
        }


        public void RemoveSet(string set) 
            => results.RemoveAll(res => res.Set == set);


        public Result this[int index] 
            => results[index];

        public ResultCollection() 
            => results = new();

        public ResultCollection(int capacity) 
            => results = new(capacity);

        public ResultCollection(IEnumerable<Result> collection)
            => results = new(collection);

        public IEnumerator<Result> GetEnumerator() 
            => ((IEnumerable<Result>)results).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => ((IEnumerable<Result>)results).GetEnumerator();

        private readonly List<Result> results;
    }
}
