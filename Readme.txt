Please Install AMD APP, AMD Latest Driver for OpenCL, AMD Bolt, and VS2012,
unzip the folder to My Documents\Visual Studio 2012\Projects\

Discrete Fourier Transform on AMD GPU
 Background
DFT transforms signal from time domain to frequency domain requiring O(n*n) operations, requiring O(?log?_2 n) with FFT butterfly algorithm.

 DFT algorithm, require O(NxN)
This example shows the method of DFT calculation, given 4 points (N=4) from a sample of periodic discrete points over time(in time domain)
X_p=?_(n=0)^(N-1)?X_n  W_N^np=?_(n=0)^(N-1)?X_n  e_N^(-j2?np)   0?p?N-1
Eg. N=4	  e^j?=cos?+jsin?=-1
P=0,   0?n?3:
X0=X0 e^(-j2? 0 0/4)+X1? e?^(-j2? 1 0/4)+ X2 e^(-j2? 2 0/4)+ X3 e^(-j2? 3 0/4)= X0+X1+X2+X3
P=1,   0?n?3:
X1=X0 e^(-j2? 0 1/4)+X1? e?^(-j2? 1 1/4)+ X2 e^(-j2? 2 1/4)+ X3 e^(-j2? 3 1/4)
X1=X0+X1e^(-j?/2)+X2e^(-j?)+X3e^(-j?3/2)
X1=X0+X1e^(-j2?/4)-X2+X3e^(-j2?3/4)
X1=X0+X1e^(-j2?/4)-X2-X3e^(-j2?/4)
P=2,   0?n?3:
X2=X0 e^(-j2? 0 2/4)+X1? e?^(-j2? 1 2/4)+ X2 e^(-j2? 2 2/4)+ X3 e^(-j2? 3 2/4)
X2=X0e^0+X1e^(-j?)+X2e^(-2j?)+X3e^(-j?3)
X2=X0- X1+X2- X3
P=3,   0?n?3:
X3=X0 e^(-j2? 0 3/4)+X1? e?^(-j2? 1 3/4)+ X2 e^(-j2? 2 3/4)+ X3 e^(-j2? 3 3/4)
X3=X0+X1e^(-j?3/2)+X2e^(-j?3)+X3e^(-j?9/2)
X3=X0+X1e^(-j2?3/4)-X2-X3e^(-j?2 3/4)
Therefore,
X0=(X0+X2)+ e^(-j2?0/4)(X1+X3)
X1=(X0-X2)+ e^(-j2?1/4)  (X1-X3)
X2=(X0+X2)+ e^(-j2?2/4)(X1+X3)
X3=(X0-X2)+ e^(-j2?3/4)  (X1-X3)
So,
For N = 4
X0=(X0+ e^(-j2?0/2)) + e^(-j2?0/4)(X1+e^(-j2?0/2)X3)
X1=(X0- e^(-j2?0/2)) + e^(-j2?1/4)  (X1-e^(-j2?0/2)X3)
X2=(X0+ e^(-j2?0/2)) - e^(-j2?0/4)(X1+e^(-j2?0/2)X3)
X3=(X0- e^(-j2?0/2)) - e^(-j2?1/4)  (X1-e^(-j2?0/2)X3)
W_N^P  =e^(-j2?p/N)		W=e^(-j2?)
Its Butterfly Diagram:
x0-----+-------->W_2^0 -----------+---------> W_4^0   X0
x2------_------->?-W?_2^0 -----------+---------> W_4^1   X0
x1------+------->W_2^0                       ?-W?_4^0   X0
x3------_------->?-W?_2^0                      ?-W?_4^1   X0
x0-----+-------->W_2^0                       W_4^0   X0
x2------_------->?-W?_2^0                    W_4^1   X0
x1------+------->?-W?_2^0   -----------+---------> ?-W?_4^0   X0
x3------_------->?-W?_2^0                      ?-W?_4^1   X0

 Butterfly Diagram, require O(log2N)
Eg. N=2					
X0=X0 + e^(-j2?0/2)X1		X1=X0 - e^(-j2?0/2)X1		
x0	X1 W_2^0			x0	X1 W_2^0
x1	X1W_2^0			x1	X1W_2^0
Conclusion:
W_2^0= 1		X0=X0 + W_2^0X1	X1=X0 - W_2^0X1
Eg. N=4
x0		?+W?_2^0		?+W?_4^0	X0
x1		?-W?_2^0		?+W?_4^0	X1
x2		?+W?_2^0		?-W?_4^0	X2
x3		?-W?_2^0		?-W?_4^0	X3
x0		?+W?_2^0		?+W?_4^0	X0
x1		?-W?_2^0		?+W?_4^0	X1
x2		?+W?_2^0		?-W?_4^0	X2
x3		?-W?_2^0		?-W?_4^0	X3
x0		?+W?_2^0		?+W?_4^0	X0
x1		?-W?_2^0		?+W?_4^0	X1
x2		?+W?_2^0		?-W?_4^0	X2
x3		?-W?_2^0		?-W?_4^0	X3
x0		?+W?_2^0		?+W?_4^0	X0
x1		?-W?_2^0		?+W?_4^0	X1
x2		?+W?_2^0		?-W?_4^0	X2
x3		?-W?_2^0		?-W?_4^0	X3
Conclusion:
X0 =  (x0+W_2^0 x2)+ W_4^0 (x1+W_2^0 x3)


X1 =  (x0-W_2^0 x2)+ W_4^1 (x1-W_2^0 x3)


X2 =  (x0+W_2^0 x2)- W_4^0 (x1+W_2^0 x3)


X3 =  (x0-W_2^0 x2)- W_4^1 (x1-W_2^0 x3)


W_4^0= 1	W_4^1= -j	W_2^0= 1
For x0=0, x1=1, x2=0, x3=-1
X0=0+0+(1-1)=0, X1=0-0-j(1+1)=-2j, X2=0+0-(1-1)=0, X3=0-0+j(1+1)=2j
 In place algorithm, bit reverse

N=4	Bit	Reversed Bit	Reversed Decimal
0	00		00		0
1	01		10		2
2	10		01		1
3	11		11		3
Eg. N = {0, 1, 0, -1} -> N = {0, 0, 1, -1}



N=8	Bit	Reversed Bit	Reversed Decimal
0	000		000		0
1	001		100		4
2	010		010		2
3	011		110		6
4	100		001		1
5	101		101		5
6	110		011		3
7	111		111		7
Eg. N={2,1,-1,-3,0,1,2,1} -> N{2,0,-1,2,1,1,-3,1}


 Butterfly Algorithm

X[8] = {2,1,-1,-3,0,1,2,1}
2		2+0 2		     2+1 3				   3+0
0		2-0  2		      2-3(-j) 2+3j				    2+3j+4j(1/?2-j/(?2))
-1		-1+2  1		      2-1  1				    1+4(-j)
2		-1-2  3		      2+(-3)j  2-3j			     2-3j+(-4j)(-1/?2-j/?2)
1		1+1  2		      2+(-2)  0				      3
1		1-1  0		       0-4(-j)  4j				      2+3j-4j(1/?2-j/(?2))
-3		-3+1  -2		       2-(-2)  4				      1+4j
1		-3-1  -4		       0+(-4)j  -4j				       2-3j-4j(1/?2+j/(?2))
1/8 = 45°  i^0=1  i^2=-1  i^3=-i  i^4=1


 One Complete Example
One time domain signal discrete periodic wave decomposed to several Sin and Cos curves
f(n)=?_(k=0)^(N-1)?(x(k)e??(-j2?kn)/N? ) 
e^jx=cosX+jsinX
W_N=e^(-j2?/N)
W_N^kn=e^(-j2?kn/N)
W_N^kn=?Cos(?^(-j2?kn/N))+j?Sin(?^(-j2?kn/N))
f(x) = Sin(x)

f(x) = Cos(x)

Time Domain Wave:
2?	N=4	T=0.1s	T/N=0.1/4=0.025 f=1/T=40Hz	40 samples per second
f(k)=sin(k?/2)
k=0, f(0) = 0		k=1, f(1) = 1
k=2, f(2) = 0		k=3, f(3) = -1
Time Domain: N=(0,1,0,-1)
0?k ?N-1		0?k ?3		0?n ?3
W_2^0= 1		W_4^kn		W_4^0= 1		W_4^1= -j		W_4^2= -1		W_4^3= j
DFT:
F(n)=x(0) W_4^0n+x(1) W_4^1n+x(2) W_4^2n+x(3)W_4^3n
F(0)=x(0) W_4^00+x(1) W_4^10+x(2) W_4^20+x(3)W_4^30=0+1+0-1=0
F(1)=x(0) W_4^01+x(1) W_4^11+x(2) W_4^21+x(3) W_4^31
=x(0)+x(1) W_4^1+x(2) W_4^2+x(3)W_4^3=0-j+0-j=-2j
F(2)=x(0) W_4^02+x(1) W_4^12+x(2) W_4^22+x(3)W_4^32=0-1+0+1=0
F(3)=x(0) W_4^03+x(1) W_4^13+x(2) W_4^23+x(3)W_4^33=0+j+0+j=2j
F(0)=0, F(1)=-2j,F(2)=0,F(3)=2j
FFT:
Because of:
X0 =  (x0+W_2^0 x2)+ W_4^0 (x1+W_2^0 x3)
X1 =  (x0-W_2^0 x2)+ W_4^1 (x1-W_2^0 x3)
X2 =  (x0+W_2^0 x2)- W_4^0 (x1+W_2^0 x3)
X3 =  (x0-W_2^0 x2)- W_4^1 (x1-W_2^0 x3)
Therefore:
X0=0+0+(1-1)=0	X1=0-0-j(1+1)=-2j
X2=0+0-(1-1)=0	X3=0-0+j(1+1)=2j
F(0)=0, F(1)=-2j,F(2)=0,F(3)=2j
Frequency function:
f=f0^* n	f0^ =1/T=10Hz
Phase: real + j * imaginary	Magnitude =  ?(?real?^2+?imaginary?^2 )		F(n) = n * 10Hz
F(0) = 0Hz	F(1) = 10Hz	F(2) = 20Hz	F(3) = 30Hz
M(0) = 0	M(1) = 2	M(2) = 0	M(3) = 2
F(n) -> M(n)
Valid Frequency: 40/2 = 20Hz	Any spikes above 20Hz is invalid
Frequency Domain:

 Lucas Lehmer Test using FFT (CPU)
 Given cl_int P, calculate MP = 2^P
 isPM function: (define m as an arbitrary big integer number) 
//The key is to use BigInteger(C# .Net 4.0 System.Numerics.BigInterger, C++ BigIntegerLibrary.hh)
        bool isMP() (cl_int p)//c++
	{
		const BigInteger base = 2;
		if(p%2==0) return (p==2);
		else
		{
			for(cl_int i = 3; i<= sqrt(p);i += 2)
				if(p % i == 0) return false;
			BigInteger m = BigIntegerPow(base,p)-1;
			BigInteger s = 4;
			for(cl_int i = 3; i<= p; i++)
				s  = (s*s -2) % m; //Multiplication Step
			return s==0;
		}
};
 Use FFT and Schönhage–Strassen_algorithm on Multiplication Step reduce Big O  complexity 
from O(N*N) to O(N logN log log N)
Steps:
 Input A: 1234 Input B: 5678
 PadA: 43210000 PadB: 87650000
 fA: FFT(PadA) fB:FFT(PadB)
 PointeweiseMultiplication: fA * fB
 InverseFFT(PointeweiseMultiplication) * N
 Difficulties of implementation on GPU is OpenCL does not natively support BigInteger type
 AMD FFT(GPU) can be used to calculate massive integer multiplication
 Other methods for massive integer multiplication:
Toom Cook: (C# .NET V4.0)

public static BigInteger ToomCook_2_Multiply(BigInteger a, BigInteger b)
        {
            String m = a.ToString();
            String n = b.ToString();
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
                rr[i] = PP[i] * qq[i];
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
                    rr[i] = PP[i] * qq[i];//most expensive mutiplication
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
  result = rr[0] + rr[1] * tmp + rr[2] * tmp * tmp + rr[3] * tmp * tmp * tmp + rr[4] * tmp * tmp * tmp * tmp;
                Console.WriteLine("ToomCook 3 Result: " + result);
            }
            else if (simbol == 1)
            {
                result = ToomCook_2_Multiply(a, b);
            }
            return result;
        }
Schonhage Strassen with FFT: (C# .NET V4.0 AForge.Math Library)
static public BigInteger Schonhage_Strassen_Fast_Fourier_Transform(AForge.Math.Complex[] ldata, AForge.Math.Complex[] rdata)
        {
            AForge.Math.Complex[] redata = new AForge.Math.Complex[ldata.Length];
            FourierTransform.FFT(ldata, FourierTransform.Direction.Forward);
            FourierTransform.FFT(rdata, FourierTransform.Direction.Forward);
            for (int i = 0; i < ldata.Length; i++)
            {
                redata[i] = AForge.Math.Complex.Multiply(ldata[i] * ldata.Length, rdata[i] * rdata.Length);
            }
            FourierTransform.FFT(redata, FourierTransform.Direction.Backward);
            // clean up 0 errors
            for (int i = 0; i < redata.Length; i++)
            {
                redata[i].Re = Math.Round(redata[i].Re);
                redata[i].Im = Math.Round(redata[i].Im);
            }
            BigInteger result = 0;
            for (int i = 0; i < redata.Length; i++)
            {
                result += (BigInteger)redata[i].Re * BigInteger.Pow(10, i);
            }
            result = result / redata.Length;
            Console.WriteLine("Schonhage_Strassen_FFT Answer:" + result);
            return result;
        }
 Lucas Lehmer Test using AMD FFT(GPU)
 References:
http://www.youtube.com/watch?v=D5ueRUyCP58
http://www.youtube.com/watch?v=vlFdVYAXIxg
http://www.dspguide.com/ch8/4.htm
http://en.wikipedia.org/wiki/Fast_Fourier_transform
http://en.wikipedia.org/wiki/Butterfly_diagram
http://www.alwayslearn.com/dft%20and%20fft%20tutorial/DFTandFFT_TheDFT.html
http://developer.amd.com/resources/documentation-articles/articles-whitepapers/opencl-optimization-case-study-fast-fourier-transform-part-1/
http://en.wikipedia.org/wiki/Lucas%E2%80%93Lehmer_primality_test
http://web.cecs.pdx.edu/~maier/cs584/Lectures/lect07b-11-MG.pdf
http://en.wikipedia.org/wiki/Toom%E2%80%93Cook_multiplication
http://en.wikipedia.org/wiki/Sch%C3%B6nhage%E2%80%93Strassen_algorithm
http://www.bealto.com/mp-opencl_ssa.html
http://www.mpi-inf.mpg.de/~csaha/intMult.pdf
https://mattmccutchen.net/bigint/
http://developer.amd.com/tools-and-sdks/heterogeneous-computing/amd-accelerated-parallel-processing-app-sdk/bolt-c-template-library/
http://hsa-libraries.github.io/Bolt/html/index.html


Mao Weiqing	Pert, WA		December 27, 2013

