using System;
using System.Collections.Generic;
using System.Linq;

namespace Communicator.Common.RSA
{
    public class PrimeNumberGenerator
    {
        public List<int> PrimeNumbers = new List<int>();
        private Random _random = new Random();

        private int _numberOfPrimes = 500; 
        public PrimeNumberGenerator()
        {
            var numbers = Enumerable.Range(2, (int)(_numberOfPrimes * (Math.Log(_numberOfPrimes) + Math.Log(Math.Log(_numberOfPrimes)) - 0.5)))
                        .AsParallel()
                        .WithDegreeOfParallelism(Environment.ProcessorCount) 
                        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .WithMergeOptions(ParallelMergeOptions.NotBuffered) 
                        .Where(x => Enumerable.Range(2, x - 2).All(y => x % y != 0))
                        .TakeWhile((n, index) => index < _numberOfPrimes).OrderBy(x => x); 
            PrimeNumbers = numbers.ToList();
        }

        public int GetRandom()
        {
            return PrimeNumbers[_random.Next(0, PrimeNumbers.Count)];
        }
    }
}
