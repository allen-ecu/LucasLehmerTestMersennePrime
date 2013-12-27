using Skyiv;
using System;
using System.Numerics;

namespace isMersennePrime
{
    static class BigIntegerMultiplicationFFT
    {
        static public BigInteger BigIntegerMultiplication_Fast_Fourier_Transform(String N)
        {
            String bn = N;
            int leng = N.Length;
            BigInteger[] m = new BigInteger[leng];
            BigInteger[] n = new BigInteger[leng];
            for (int i = 0; i < leng; i++)
            {
                m[i] = Utility.BigIntegerFromString(N.Substring(i, 1));
                n[i] = m[i];
            }

            BigInteger result = 0;
            int len = m.Length;
            if (len == n.Length)
            {
                BigInteger[] product = new BigInteger[len * 2];
                Skyiv.Numeric.BigArithmetic.MultiplyBigInteger(product, m, len, n, len);
                BigInteger[] sub = new BigInteger[len * 2];
                for (int i = 1; i < product.Length; i++)
                {
                    result += BigInteger.Pow(10, i - 1) * product[i];
                    sub[i - 1] = result;
                }
                //Console.WriteLine("BigInteger Multiplication FFT Answer: " + result);
            }
            else
            {
                Console.WriteLine("Error: two arrays need the same length!");
            }
            return result;
        }

        static public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

    }
}
