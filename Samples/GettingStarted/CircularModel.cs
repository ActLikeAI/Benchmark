using ActLikeAI.Benchmark;
using System.Threading;

namespace GettingStarted
{
    public class CircularModel
    {
        [Benchmark(RunCount = 30, Name = "HighAsync")]
        public void DetachedA() 
            => Thread.Sleep(55);


        [Benchmark(Name = "SmallSecondary")]
        public void DetachedB() 
            => Thread.Sleep(75);


        [Benchmark(RunCount = 50)]
        public void SemiDetached() 
            => Thread.Sleep(25);


        [Benchmark]
        public void Contact() 
            => Thread.Sleep(130);


        [Benchmark]
        public void Overcontact() 
            => Thread.Sleep(100);


        //Methods with arguments are ignored.
        [Benchmark]
        public void WrongSignature(string s, double x)
        {
            Thread.Sleep(10);
        }
    }
}
