using Communicator.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Communicator.Common.RSA
{
    public class RSAService
    {
        private int _d;
        private int _e;
        private Encoding _encoding = Encoding.UTF8;
        private int _multiplicationResult;
        private Random _random = new Random();
        private int _totient;


        
        public RSAService()
        {
            var primes = GetRandomPrimes(); 
            _multiplicationResult = primes.Item1 * primes.Item2; 
            _totient = (primes.Item1 - 1) * (primes.Item2 - 1);
            var coprimes = FindAllCoprimes(_totient);
            do
            {
                _e = coprimes[_random.Next(0, coprimes.Count)]; 
            } while (_totient % _e == 0);

            Console.WriteLine($"{DateTime.Now} - Klucz publiczny: (E={_e}, N={_multiplicationResult})");
            _d = ModularInverse(_e, _totient); 
            Console.WriteLine($"{DateTime.Now} - Klucz prywatny: (D={_d}, N={_multiplicationResult})");
        }
        

        public string Decrypt(BigInteger[] encrypted)
        {
            byte[] bytes = encrypted.Select(bi => (byte)BigInteger.ModPow(bi, _d, _multiplicationResult))
                                    .ToArray();
            return _encoding.GetString(bytes);
        }


        public BigInteger[] Encrypt(string text, PublicKey key)
        {
            byte[] bytes = _encoding.GetBytes(text);
            return bytes.Select(b => BigInteger.ModPow(b, key.E, key.N)).ToArray();
        }


        private List<int> FindAllCoprimes(int number)
        {
            var coprimes = Enumerable.Range(2, number - 1).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .WithMergeOptions(ParallelMergeOptions.NotBuffered).Where(x => IsCoprime(number, x)).OrderBy(z => z).ToList();
            return coprimes;
        }


        public PublicKey GetPublicKey()
        {
            return new PublicKey() { E = _e, N = _multiplicationResult };
        }


        public bool IsCoprime(int value1, int value2)
        {
            return CalculateGreatestCommonDivisor(value1, value2) == 1;
        }


        private int CalculateGreatestCommonDivisor(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }
            else return CalculateGreatestCommonDivisor(b, a % b);
        }


        private Tuple<int, int> GetRandomPrimes()
        {
            var primeNumberGenerator = new PrimeNumberGenerator();
            return new Tuple<int, int>(primeNumberGenerator.GetRandom(), primeNumberGenerator.GetRandom());
        }

        private int ModularInverse(int a, int n)
        {
            int i = n, v = 0, d = 1;
            while (a > 0)
            {
                int t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }
    }
}
