using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Skyiv;

namespace isMersennePrime
{
    static class Toom_Cook
    {
        public static BigInteger ToomCook_2_Multiply(BigInteger a, BigInteger b)
        {
            String m = a.ToString();
            String n = b.ToString();
            //Console.WriteLine("input a: " + m);
            //Console.WriteLine("input b: " + n);
            //Splitting 2
            int mlength = m.Length;
            int nlength = n.Length;
            String m1 = m.Substring(0, mlength / 2);
            String m0 = m.Substring(mlength / 2, mlength % 2 == 0 ? mlength / 2 : mlength / 2 + 1);
            String n1 = n.Substring(0, nlength / 2);
            String n0 = n.Substring(nlength / 2, nlength % 2 == 0 ? nlength / 2 : nlength / 2 + 1);
            BigInteger[] PP = new BigInteger[3];
            BigInteger[] qq = new BigInteger[3];
            // Fast Evaluation
            PP[0] = Utility.BigIntegerFromString(m0);
            qq[0] = Utility.BigIntegerFromString(n0);
            PP[1] = Utility.BigIntegerFromString(m1) + Utility.BigIntegerFromString(m0);
            qq[1] = Utility.BigIntegerFromString(n1) + Utility.BigIntegerFromString(n0);
            PP[2] = Utility.BigIntegerFromString(m1);
            qq[2] = Utility.BigIntegerFromString(n1);
            // Posintwise Multiplication
            BigInteger[] rr = new BigInteger[3];
            for (int i = 0; i < 3; i++)
            {
                //Console.Write("PP" + i + ":" + PP[i]+",");
                //Console.Write("qq" + i + ":" + qq[i]+",");
                rr[i] = PP[i] * qq[i];
                //Console.WriteLine("rr"+i+":"+rr[i] + ",");
            }
            // Interpolation
            rr[1] = rr[1] - rr[0] - rr[2];
            //Console.WriteLine("rr[1]"+rr[1]);
            // Recomposition
            BigInteger tmp = BigInteger.Pow(10, m0.Length);
            BigInteger result = rr[0] + rr[1] * tmp + rr[2] * tmp * tmp;
            Console.WriteLine("ToomCook 2 Result: " + result);
            return result;
        }

        public static BigInteger ToomCook_3_Multiply(BigInteger a, BigInteger b)
        {
            // warning: a and b must be the same length
            BigInteger result = 0;
            Console.WriteLine("Toom Cook 3:");
            String m = a.ToString();
            String n = b.ToString();
            //Console.WriteLine("BigInteger a: " + m);
            //Console.WriteLine("BigInteger b: " + n);
            int mlength = m.Length;
            int nlength = n.Length;
            int d = 3;
            int v = d * 2 - 1;
            // Splitting 3
            int inc = 0;
            int num = mlength / d + inc;
            while (!Utility.IsPowerOfTwo(num))
            {
                inc = inc + 1;
                num = mlength / d + inc;
            }
            int ind = 0;
            int mun = nlength / d + ind;
            while (!Utility.IsPowerOfTwo(mun))
            {
                ind = ind + 1;
                mun = nlength / d + ind;
            }
            int simbol = nlength / mun;

            if (simbol == 2)
            {
                String m2 = m.Substring(0, mlength - num - num);
                String m1 = m.Substring(mlength - num - num, num);
                String m0 = m.Substring(mlength - num, num);
                String n2 = n.Substring(0, nlength - mun - mun);
                String n1 = n.Substring(nlength - mun - mun, mun);
                String n0 = n.Substring(nlength - mun, mun);
                //Console.WriteLine("Splitting:");
                //Console.WriteLine("m2: " + m2 + " m1: " + m1 + " m0: " + m0);
                //Console.WriteLine("n2: " + n2 + " n1: " + n1 + " n0: " + n0);
                // Fast Evaluation
                BigInteger[] PP = new BigInteger[v];
                BigInteger[] qq = new BigInteger[v];
                BigInteger tmpP = Utility.BigIntegerFromString(m0) + Utility.BigIntegerFromString(m2);
                BigInteger tmpQ = Utility.BigIntegerFromString(n0) + Utility.BigIntegerFromString(n2);
                PP[0] = Utility.BigIntegerFromString(m0);
                qq[0] = Utility.BigIntegerFromString(n0);
                PP[1] = tmpP + Utility.BigIntegerFromString(m1);
                qq[1] = tmpQ + Utility.BigIntegerFromString(n1);
                PP[2] = tmpP - Utility.BigIntegerFromString(m1);
                qq[2] = tmpQ - Utility.BigIntegerFromString(n1);
                PP[3] = (PP[2] + Utility.BigIntegerFromString(m2)) * 2 - Utility.BigIntegerFromString(m0);
                qq[3] = (qq[2] + Utility.BigIntegerFromString(n2)) * 2 - Utility.BigIntegerFromString(n0);
                PP[4] = Utility.BigIntegerFromString(m2);
                qq[4] = Utility.BigIntegerFromString(n2);
                // Posintwise Multiplication
                BigInteger[] rr = new BigInteger[v];
                // Console.WriteLine("Fast Evaluation:");
                for (int i = 0; i < v; i++)
                {
                    //Console.Write("pp" + i + ":" + PP[i] + ",");
                    //Console.Write("qq" + i + ":" + qq[i] + ",");
                    rr[i] = PP[i] * qq[i];//most expensive mutiplication
                    //Console.WriteLine("rr" + i + ":" + rr[i] + ",");
                }
                // Interpolation
                BigInteger r0 = rr[0];
                BigInteger r4 = rr[4];
                BigInteger r3 = (rr[3] - rr[1]) / 3;
                BigInteger r1 = (rr[1] - rr[2]) / 2;
                BigInteger r2 = rr[2] - rr[0];
                r3 = (r2 - r3) / 2 + 2 * rr[4];
                r2 = r2 + r1 - r4;
                r1 = r1 - r3;
                // save the value to rr[]
                rr[0] = r0;
                rr[1] = r1;
                rr[2] = r2;
                rr[3] = r3;
                rr[4] = r4;
                // Recomposition
                BigInteger tmp = BigInteger.Pow(10, m0.Length);
                //Console.WriteLine(tmp + " i.e. 10^" + m0.Length);
                result = rr[0] + rr[1] * tmp + rr[2] * tmp * tmp + rr[3] * tmp * tmp * tmp + rr[4] * tmp * tmp * tmp * tmp;
                Console.WriteLine("ToomCook 3 Result: " + result);
                //Console.WriteLine("Finished. ");
            }
            else if (simbol == 1)
            {
                result = ToomCook_2_Multiply(a, b);
            }
            return result;
        }

    }
}
