using AForge.Math;
using Skyiv;
using System;
using System.Numerics;

namespace isMersennePrime
{
    static class Schonhage_Strassen_FFT
    {
        static public AForge.Math.Complex[] data_builder(BigInteger a)
        {
            //input 1234
            //return 43210000
            //2*n
            //return AForge.Math.Complex[] data
            String bigNum = a.ToString();
            int len = bigNum.Length;
            int i = len;
            int num = len + i;
            while (!Utility.IsPowerOfTwo(num))
            {
                i = i + 1;
                num = len + i;
            }
            int length = len + i;
            AForge.Math.Complex[] data = new AForge.Math.Complex[length];

            //Console.WriteLine("");
            //Console.WriteLine("Input:" + a);
            //Console.WriteLine("Output Builder:");
            for (int j = 0; j < len; j++)
            {
                data[len - j - 1] = new AForge.Math.Complex(Convert.ToDouble(bigNum.Substring(j, 1)), 0.0);
            }
            return data;
        }

        static public BigInteger Schonhage_Strassen_Fast_Fourier_Transform(AForge.Math.Complex[] ldata, AForge.Math.Complex[] rdata)
        {
            AForge.Math.Complex[] redata = new AForge.Math.Complex[ldata.Length];

            //Stopwatch sw1 = new Stopwatch();
            //Stopwatch sw2 = new Stopwatch();
            //sw1.Start();
            FourierTransform.FFT(ldata, FourierTransform.Direction.Forward);
            //sw1.Stop();
            //TimeSpan ts1 = sw1.Elapsed;
            //String out1 = ts1.TotalSeconds.ToString();
            //Console.WriteLine("");
            //Console.WriteLine("LEFT:");
            //for (int i = 0; i < ldata.Length; i++)
            //{
            //    Console.Write(ldata[i]* ldata.Length+",");
            //}
            //Console.WriteLine("Time: " + out1);

            //sw2.Start();
            FourierTransform.FFT(rdata, FourierTransform.Direction.Forward);
            //sw2.Stop();
            //TimeSpan ts2 = sw2.Elapsed;
            //String out2 = ts2.TotalSeconds.ToString();
            //Console.WriteLine("RIGHT:");
            //for (int i = 0; i < rdata.Length; i++)
            //{
            //    Console.Write(rdata[i]* rdata.Length+",");
            //}
            //Console.WriteLine("Time: " + out2);

            //Console.WriteLine("Result of Multiply:");
            for (int i = 0; i < ldata.Length; i++)
            {
                redata[i] = AForge.Math.Complex.Multiply(ldata[i] * ldata.Length, rdata[i] * rdata.Length);
                //Console.Write(redata[i]+",");
            }
            //Console.WriteLine("");
            //Console.WriteLine("Inverse:");
            FourierTransform.FFT(redata, FourierTransform.Direction.Backward);

            // clean up 0 errors
            for (int i = 0; i < redata.Length; i++)
            {
                redata[i].Re = Math.Round(redata[i].Re);
                redata[i].Im = Math.Round(redata[i].Im);
            }

            //for (int i = 0; i < redata.Length; i++)
            //{
            //    Console.Write(redata[i] / redata.Length + ",");
            //}

            BigInteger result = 0;
            for (int i = 0; i < redata.Length; i++)
            {
                result += (BigInteger)redata[i].Re * BigInteger.Pow(10, i);
            }

            result = result / redata.Length;
            //Console.WriteLine("");
            Console.WriteLine("Schonhage_Strassen_FFT Answer:" + result);
            return result;
        }

    }
}
