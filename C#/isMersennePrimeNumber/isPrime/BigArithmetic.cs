using System;
using System.Diagnostics;

namespace Skyiv.Numeric
{
  /// <summary>
  ///
  /// </summary>
  static class BigArithmetic
  {
    static readonly byte Len = 2; 
    static readonly byte Base = (byte)Math.Pow(10, Len); 
    static readonly byte MaxValue = (byte)(Base - 1);    

    /// </summary>
    /// Complex FFT
    /// <param name="data">data[1..2*nn]\</param>
    /// <param name="isInverse"> 1/nn</param>
    public static void ComplexFFT(double[] data, bool isInverse)
    {
      int n = data.Length - 1; // 2^n
      int nn = n >> 1;         // nn 
      for (int i = 1, j = 1; i < n; i += 2) 
      {
        if (j > i)
        {
          Utility.Swap(ref data[j], ref data[i]);
          Utility.Swap(ref data[j + 1], ref data[i + 1]);
        }
        int m = nn;
        for (; m >= 2 && j > m; m >>= 1) j -= m;
        j += m;
      }
      for (int mmax = 2, istep = 4; n > mmax; mmax = istep) // log2(nn)
      {
        istep = mmax << 1; // 
        double theta = (isInverse ? -2 : 2) * Math.PI / mmax;
        double wtemp = Math.Sin(0.5 * theta);
        double wpr = -2 * wtemp * wtemp;
        double wpi = Math.Sin(theta);
        double wr = 1;
        double wi = 0;
        for (int m = 1; m < mmax; m += 2)
        {
          for (int i = m; i <= n; i += istep)
          {
            int j = i + mmax; //  Danielson-Lanczos
            double tempr = wr * data[j] - wi * data[j + 1];
            double tempi = wr * data[j + 1] + wi * data[j];
            data[j] = data[i] - tempr;
            data[j + 1] = data[i + 1] - tempi;
            data[i] += tempr;
            data[i + 1] += tempi;
          }
          wr = (wtemp = wr) * wpr - wi * wpi + wr; // 
          wi = wi * wpr + wtemp * wpi + wi;
        }
      }
    }

    /// <summary>
    /// Real FFT
    /// </summary>
    /// <param name="data">data[1..n]</param>
    /// <param name="isInverse"> 1/n</param>
    public static void RealFFT(double[] data, bool isInverse)
    {
      int n = data.Length - 1; // 2^n
      if (!isInverse) ComplexFFT(data, isInverse); 
      double theta = (isInverse ? -2 : 2) * Math.PI / n; 
      double wtemp = Math.Sin(0.5 * theta);
      double wpr = -2 * wtemp * wtemp;
      double wpi = Math.Sin(theta);
      double wr = 1 + wpr;
      double wi = wpi;
      double c1 = 0.5;
      double c2 = isInverse ? 0.5 : -0.5;
      int n3 = n + 3;
      int n4 = n >> 2;
      for (int i = 2; i <= n4; i++)
      {
        int i1 = i + i - 1, i2 = i1 + 1, i3 = n3 - i2, i4 = i3 + 1;
        double h1r = c1 * (data[i1] + data[i3]); 
        double h1i = c1 * (data[i2] - data[i4]); 
        double h2r = -c2 * (data[i2] + data[i4]);
        double h2i = c2 * (data[i1] - data[i3]);
        data[i1] = h1r + wr * h2r - wi * h2i; 
        data[i2] = h1i + wr * h2i + wi * h2r; 
        data[i3] = h1r - wr * h2r + wi * h2i;
        data[i4] = -h1i + wr * h2i + wi * h2r;
        wr = (wtemp = wr) * wpr - wi * wpi + wr; 
        wi = wi * wpr + wtemp * wpi + wi;
      }
      double tmp = data[1];
      if (!isInverse)
      {
        data[1] = tmp + data[2]; 
        data[2] = tmp - data[2];
      }
      else
      {
        data[1] = c1 * (tmp + data[2]);
        data[2] = c1 * (tmp - data[2]);
        ComplexFFT(data, isInverse); 
      }
    }

    /// <summary>
    /// Comparen x[0..n-1] with y[0..n-1]
    /// </summary>
    /// <param name="x"> x[0..n-1]</param>
    /// <param name="y"> y[0..n-1]</param>
    /// <param name="n"> x y </param>
    /// <returns>：-1:lessthan 1:more than 0:equal</returns>
    public static int Compare(byte[] x, byte[] y, int n)
    {
      Debug.Assert(x.Length >= n && y.Length >= n);
      for (int i = 0; i < n; i++)
        if (x[i] != y[i])
          return (x[i] < y[i]) ? -1 : 1;
      return 0;
    }

    /// <summary>
    /// Bitwise Negative Operation
    /// </summary>
    /// <param name="data">data[0..n-1]</param>
    /// <param name="n">data </param>
    /// <returns> data[0..n-1]</returns>
    public static byte[] Negative(byte[] data, int n)
    {
      Debug.Assert(data.Length >= n);
      for (int k = Base, i = n - 1; i >= 0; i--)
        data[i] = (byte)((k = MaxValue + k / Base - data[i]) % Base);
      return data;
    }

    /// <summary>
    /// minuend[0..n-1] minue subtrahend[0..n-1]，return difference[0..n-1]
    /// </summary>
    /// <param name="difference"difference[0..n-1]</param>
    /// <param name="minuend">minuend[0..n-1]</param>
    /// <param name="subtrahend">subtrahend[0..n-1]</param>
    /// <param name="n">minuend  subtrahend </param>
    /// <returns> difference[0..n-1]</returns>
    public static byte[] Subtract(byte[] difference, byte[] minuend, byte[] subtrahend, int n)
    {
      Debug.Assert(minuend.Length >= n && subtrahend.Length >= n && difference.Length >= n);
      for (int k = Base, i = n - 1; i >= 0; i--)
        difference[i] = (byte)((k = MaxValue + k / Base + minuend[i] - subtrahend[i]) % Base);
      return difference;
    }

    /// <summary>
    /// Addition augend[0..n-1] and addend[0..n-1] return sum[0..n]
    /// </summary>
    /// <param name="sum">sum[0..n]</param>
    /// <param name="augend"> augend[0..n-1]</param>
    /// <param name="addend"> addend[0..n-1]</param>
    /// <param name="n"> augend  addend </param>
    /// <returns>sum[0..n]</returns>
    public static byte[] Add(byte[] sum, byte[] augend, byte[] addend, int n)
    {
      Debug.Assert(augend.Length >= n && addend.Length >= n && sum.Length >= n + 1);
      int k = 0;
      for (int i = n - 1; i >= 0; i--)
        sum[i + 1] = (byte)((k = k / Base + augend[i] + addend[i]) % Base);
      sum[0] += (byte)(k / Base);
      return sum;
    }

    /// <summary>
    /// add。augend[0..n-1]  and addend return sum[0..n]
    /// </summary>
    /// <param name="sum">sum[0..n]</param>
    /// <param name="augend"> augend[0..n-1]</param>
    /// <param name="n"> augend </param>
    /// <param name="addend"> addend</param>
    /// <returns>sum[0..n]</returns>
    public static byte[] Add(byte[] sum, byte[] augend, int n, byte addend)
    {
      Debug.Assert(augend.Length >= n && sum.Length >= n + 1);
      int k = Base * addend;
      for (int i = n - 1; i >= 0; i--)
        sum[i + 1] = (byte)((k = k / Base + augend[i]) % Base);
      sum[0] += (byte)(k / Base);
      return sum;
    }

    /// <summary>
    /// Devide dividend[0..n-1] divide divisor return quotient[0..n-1]
    /// </summary>
    /// <param name="quotient"> quotient[0..n-1]</param>
    /// <param name="dividend"> dividend[0..n-1]</param>
    /// <param name="n"> dividend </param>
    /// <param name="divisor"> divisor</param>
    /// <returns>quotient[0..n-1]</returns>
    public static byte[] Divide(byte[] quotient, byte[] dividend, int n, byte divisor)
    {
      Debug.Assert(quotient.Length >= n && dividend.Length >= n);
      for (int r = 0, k = 0, i = 0; i < n; i++, r = k % divisor)
        quotient[i] = (byte)((k = Base * r + dividend[i]) / divisor);
      return quotient;
    }

    /// <summary>
    /// Multiplication multiplicand[0..n-1] and multiplier[0..m-1] return product[0..n+m-1]
    /// </summary>
    /// <param name="product"> product[0..n+m-1]</param>
    /// <param name="multiplicand"> multiplicand[0..n-1]</param>
    /// <param name="n"> multiplicand </param>
    /// <param name="multiplier"> multiplier[0..m-1]</param>
    /// <param name="m"> multiplier </param>
    /// <returns> product[0..n+m-1]</returns>
    public static byte[] Multiply(byte[] product, byte[] multiplicand, int n, byte[] multiplier, int m)
    {
      int mn = m + n, nn = 1;
      Debug.Assert(product.Length >= mn && multiplicand.Length >= n && multiplier.Length >= m);
      while (nn < mn) nn <<= 1; 
      double[] a = new double[nn + 1], b = new double[nn + 1];
      for (int i = 0; i < n; i++) a[i + 1] = multiplicand[i];
      for (int i = 0; i < m; i++) b[i + 1] = multiplier[i];
      RealFFT(a, false); 
      RealFFT(b, false);
      b[1] *= a[1]; 
      b[2] *= a[2];
      for (int i = 3; i <= nn; i += 2)
      {
        double t = b[i];
        b[i] = t * a[i] - b[i + 1] * a[i + 1];
        b[i + 1] = t * a[i + 1] + b[i + 1] * a[i];
      }
      RealFFT(b, true); 
      byte[] bs = new byte[nn + 1];
      long cy = 0; 
      for (int i = nn, n2 = nn / 2; i >= 1; i--)
      {
        long t = (long)(b[i] / n2 + cy + 0.5);
        bs[i] = (byte)(t % Base); 
        cy = t / Base;
      }
      if (cy >= Base) throw new OverflowException("FFT Multiply");
      bs[0] = (byte)cy;
      Array.Copy(bs, product, n + m);
      return product;
    }

    /// <summary>
    /// Multiplication BigInteger multiplicand[0..n-1] and multiplier[0..m-1] return product[0..n+m-1]
    /// </summary>
    /// <param name="product"> product[0..n+m-1]</param>
    /// <param name="multiplicand"> multiplicand[0..n-1]</param>
    /// <param name="n"> multiplicand </param>
    /// <param name="multiplier"> multiplier[0..m-1]</param>
    /// <param name="m"> multiplier </param>
    /// <returns> product[0..n+m-1]</returns>
    public static System.Numerics.BigInteger[] MultiplyBigInteger(System.Numerics.BigInteger[] product, System.Numerics.BigInteger[] multiplicand, int n, System.Numerics.BigInteger[] multiplier, int m)
    {
        int mn = m + n, nn = 1;
        Debug.Assert(product.Length >= mn && multiplicand.Length >= n && multiplier.Length >= m);
        while (nn < mn) nn <<= 1;
        double[] a = new double[nn + 1], b = new double[nn + 1];
        for (int i = 0; i < n; i++) a[i + 1] = (double)multiplicand[i];
        for (int i = 0; i < m; i++) b[i + 1] = (double)multiplier[i];
        RealFFT(a, false); 
        RealFFT(b, false);
        b[1] *= a[1];
        b[2] *= a[2];
        for (int i = 3; i <= nn; i += 2)
        {
            double t = b[i];
            b[i] = t * a[i] - b[i + 1] * a[i + 1];
            b[i + 1] = t * a[i + 1] + b[i + 1] * a[i];
        }
        RealFFT(b, true); 
        System.Numerics.BigInteger[] bs = new System.Numerics.BigInteger[nn + 1];
        System.Numerics.BigInteger cy = 0; 
        for (int i = nn, n2 = nn / 2; i >= 1; i--)
        {
            System.Numerics.BigInteger t = (System.Numerics.BigInteger)(Math.Round(b[i]) / n2 + 0.5);
            bs[i] = t; 
            cy = t / Base;
        }
        if (cy >= Base) throw new OverflowException("FFT Multiply");
        bs[0] = (System.Numerics.BigInteger)cy;
        Array.Copy(bs, product, n + m);
        return product;
    }

    /// <summary>
    /// Divide dividend[0..n-1] divide divisor[0..m-1]，m ≤ n，
    /// return quotient[0..n-m]， remainder[0..m-1]。
    /// </summary>
    /// <param name="quotient"> quotient[0..n-m]</param>
    /// <param name="remainder"> remainder[0..m-1]</param>
    /// <param name="dividend"> dividend[0..n-1]</param>
    /// <param name="n"> dividend </param>
    /// <param name="divisor"> divisor[0..m-1]</param>
    /// <param name="m"> divisor </param>
    /// <returns> quotient[0..n-m]</returns>
    public static byte[] DivRem(byte[] quotient, byte[] remainder, byte[] dividend, int n, byte[] divisor, int m)
    {
      Debug.Assert(m <= n && dividend.Length >= n && divisor.Length >= m && quotient.Length >= n - m + 1 && remainder.Length >= m);
      int MACC = 3;
      byte[] s = new byte[n + MACC], t = new byte[n - m + MACC + n];
      Inverse(s, n - m + MACC, divisor, m); // s = 1 / divisor
      Array.Copy(Multiply(t, s, n - m + MACC, dividend, n), 1, quotient, 0, n - m + 1); // quotient = dividend / divisor
      Array.Copy(Multiply(t, quotient, n - m + 1, divisor, m), 1, s, 0, n); //  s = quotient * divisor
      Subtract(s, dividend, s, n); // s = dividend - quotient * divisor
      Array.Copy(s, n - m, remainder, 0, m);
      if (Compare(remainder, divisor, m) >= 0) 
      {
        Subtract(remainder, remainder, divisor, m);
        Add(s, quotient, n - m + 1, 1);
        Array.Copy(s, 1, quotient, 0, n - m + 1);
      }
      return quotient;
    }

    /// <summary>
    /// Inverse
    /// </summary>
    /// <param name="inverse"> inverse[0..n-1]，在 inverse[0]</param>
    /// <param name="n"> inverse </param>
    /// <param name="data"> data[0..m-1]，data[0] > 0， data[0] </param>
    /// <param name="m"> data </param>
    /// <returns> inverse[0..n-1]，在 inverse[0] </returns>
    public static byte[] Inverse(byte[] inverse, int n, byte[] data, int m)
    {
      Debug.Assert(inverse.Length >= n && data.Length >= m);
      InitialValue(inverse, n, data, m, false);
      if (n == 1) return inverse;
      byte[] s = new byte[n], t = new byte[n + n];
      for (; ; ) // Norton: inverse = inverse * ( 2 - data * inverse )  =>  inverse = 1 / data
      {
        Array.Copy(Multiply(t, inverse, n, data, m), 1, s, 0, n); // s = data * inverse
        Negative(s, n);                                         // s = -(data * inverse)
        s[0] -= (byte)(Base - 2);                             // s = 2 - data * inverse
        Array.Copy(Multiply(t, s, n, inverse, n), 1, inverse, 0, n); // inverse = inverse * s
        int i = 1;
        for (; i < n - 1 && s[i] == 0; i++) ; // 
        if (i == n - 1) return inverse; //  inverse = 1 / data
      }
    }

    /// <summary>
    /// sqrt， invSqrt。invSqrt  sqrt，invSqrt
    /// </summary>
    /// <param name="sqrt"> sqrt[0..n-1]，在 sqrt[0] </param>
    /// <param name="invSqrt"> invSqrt[0..n-1]，在 invSqrt[0] </param>
    /// <param name="n"></param>
    /// <param name="data"> data[0..m-1]，data[0] > 0，在 data[0] </param>
    /// <param name="m"> data </param>
    /// <returns> sqrt[0..n-1]，在 sqrt[0] </returns>
    public static byte[] Sqrt(byte[] sqrt, byte[] invSqrt, int n, byte[] data, int m)
    {
      Debug.Assert(sqrt.Length >= n && invSqrt.Length >= n && data.Length >= m);
      if (n <= 1) throw new ArgumentOutOfRangeException("n", "must greater than 1");
      InitialValue(invSqrt, n, data, m, true);
      byte[] s = new byte[n], t = new byte[n + Math.Max(m, n)];
      for (; ; ) // invSqrt = invSqrt * (3 - data * invSqrt * invSqrt) / 2 => invSqrt = 1 / sqrt(data)
      {
        Array.Copy(Multiply(t, invSqrt, n, invSqrt, n), 1, s, 0, n); // s = invSqrt * invSqrt
        Array.Copy(Multiply(t, s, n, data, m), 1, s, 0, n);   // s = data * invSqrt * invSqrt
        Negative(s, n);                                     // s = -(data * invSqrt * invSqrt)
        s[0] -= (byte)(Base - 3);                         // s = 3 - data * invSqrt * invSqrt
        Divide(s, s, n, 2);                              // s = (3 - data * invSqrt * invSqrt) / 2
        Array.Copy(Multiply(t, s, n, invSqrt, n), 1, invSqrt, 0, n);   // invSqrt = invSqrt * s
        int i = 1;
        for (; i < n - 1 && s[i] == 0; i++) ; // 
        if (i < n - 1) continue; // 
        Array.Copy(Multiply(t, invSqrt, n, data, m), 1, sqrt, 0, n); // sqrt = invSqrt * data = sqrt(data)
        return sqrt;
      }
    }

    /// <summary>
    ///  u[0..n-1]: u = 1 / data or u = 1 / sqrt(data)
    /// </summary>
    /// <param name="u"> u[0..n-1]</param>
    /// <param name="n"></param>
    /// <param name="data"> data[0..m-1]</param>
    /// <param name="m"> data </param>
    /// <param name="isSqrt"></param>
    /// <returns> u[0..n-1]</returns>
    static byte[] InitialValue(byte[] u, int n, byte[] data, int m, bool isSqrt)
    {
      Debug.Assert(u.Length >= n && data.Length >= m);
      int scale = 16 / Len; // double 16 digits
      double fu = 0;
      for (int i = Math.Min(scale, m) - 1; i >= 0; i--) fu = fu / Base + data[i];
      fu = 1 / (isSqrt ? Math.Sqrt(fu) : fu);
      for (int i = 0; i < Math.Min(scale + 1, n); i++)
      {
        int k = (int)fu;
        u[i] = (byte)k;
        fu = Base * (fu - k);
      }
      return u;
    }
  }
}