using System;
using System.Numerics;

namespace Skyiv
{
  static class Utility
  {
    public static T[] Expand<T>(T[] x, int n)
    {
      T[] z = new T[n]; // assume n >= x.Length
      Array.Copy(x, 0, z, n - x.Length, x.Length);
      return z;
    }

    public static void Swap<T>(ref T x, ref T y)
    {
      T z = x;
      x = y;
      y = z;
    }

    public static bool IsPowerOfTwo(int x)
    {
        return (x != 0) && ((x & (x - 1)) == 0);
    }

    public static BigInteger BigIntegerFromString(string positiveString)
    {
        BigInteger posBigInt = 0;
        try
        {
            posBigInt = BigInteger.Parse(positiveString);
            //Console.WriteLine(posBigInt);
        }
        catch (FormatException)
        {
            Console.WriteLine("Error:Unable to convert the string '{0}' to a BigInteger value.",
                              positiveString);
        }
        return posBigInt;
    }
  }
}
