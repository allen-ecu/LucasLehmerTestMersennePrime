/***************************************************************************                                                                                     
*   Copyright 2012 - 2013 Advanced Micro Devices, Inc.                                     
*                                                                                    
*   Licensed under the Apache License, Version 2.0 (the "License");   
*   you may not use this file except in compliance with the License.                 
*   You may obtain a copy of the License at                                          
*                                                                                    
*       http://www.apache.org/licenses/LICENSE-2.0                      
*                                                                                    
*   Unless required by applicable law or agreed to in writing, software              
*   distributed under the License is distributed on an "AS IS" BASIS,              
*   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.         
*   See the License for the specific language governing permissions and              
*   limitations under the License.                                                   

***************************************************************************/                                                                                     
//Author Mao Weiqing, Perth, Western Australia, Open Source Project
//Lucas Lehmer Test for Mersenne Prime Number using FFT for Big Integer Multiplication
#include <bolt/unicode.h>
#include <bolt/cl/scan.h>
#include <bolt/cl/transform.h>
#include <bolt/statisticalTimer.h>
#include "BigIntegerLibrary.hh"
#include <string>
#include <iostream>
#include <fstream>
#include <sstream>
#include <algorithm>
#include <math.h>
#include <numeric>
#include <vector>
using namespace std;

BOLT_FUNCTOR(Functor,
struct Functor
{
	cl_int _a;
	Functor(int a) : _a(a) {};
	float operator() (const cl_int &p,const cl_int &y)
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
				s  = (s*s -2) % m;
			return s==0;
		}
	};

	BigInteger BigIntegerPow(BigInteger base, cl_int exp)
	{
	BigInteger result = 1;
    while (exp)
    {
        if (exp & 1)
            result *= base;
        exp >>= 1;
        base *= base;
    }
    return result;
	}
};
);

string ReadFromFile()
{
  string line;
  string str;
  ifstream myfile ("MPs.txt");
  if (myfile.is_open())
  {
    while (getline (myfile,line) )
    {
      //cout << line << '\n';
	  str = line;
    }
	str = str.substr(0,str.size()-1);
	//cout<<"Last Line: "<< str;
    myfile.close();
  }
  else cout << "Unable to open file"; 
  return str;
}

void SaveToFIle(cl_int data)
{
  ofstream myfile ("MPs.txt", ios::app);
  if (myfile.is_open())
  {
    myfile<<data<<",\n";
    myfile.close();
  }
  else cout << "Unable to open file";
}

void transform(cl_int start, cl_int aSize)
{
    cout << "AMD BOLT Mersenne Prime EXAMPLE \n";
	cout << "Initialize vectors...\n";
	bolt::cl::device_vector< cl_int > MPs;
	//Create device_vector and initialize it to 0
    bolt::cl::device_vector< cl_int > boltInputA(aSize), boltInputB(aSize), boltResult(aSize, 0);
    //cout << "Inclusive Scan of device_vector length " << aSize << " \n";
    //bolt::cl::device_vector< cl_int >::iterator boltIterator = bolt::cl::inclusive_scan( boltInputA.begin( ), boltInputA.end( ), boltInputB.begin( ) );
	for(int j=0; j<aSize; j++)
	{
		boltInputB[j] = j+start;
	}
	cout<<"From: "<<boltInputB[0]<<" ";
	cout<<" To: "<<boltInputB[aSize-1]<<"\n";

	//cout << "Copy BoltInputB to BoltInputA...\n";
	boltInputA = boltInputB;
	cout << "Vectors Multiplication...\n";
	Functor func(0.0);
	bolt::cl::transform( boltInputA.begin( ), boltInputA.end( ), boltInputB.begin(), boltResult.begin(), func);
	cout << "Multiplication Result...\n";
	bolt::cl::device_vector< cl_int >::iterator boltResultIterator;
	cl_int mm;
	for(cl_int i= 0; i< boltResult.size();i++)
	{
		mm = boltResult[i];
		if(mm == 1)
		{
			cout<<"M"<<i+start<<" ,";
			MPs.push_back(i+start);
			SaveToFIle(i+start);
		}
	}
	cout << "\nFinished...\n";
	
	//timer
	//biginteger
	
	return;
};

int _tmain( int argc, _TCHAR* argv[ ] )
{
	try {

		cl_int length = 1000;// numbers quantity to calculate
		string lastNum = ReadFromFile();
		if(lastNum  == "")
		transform(1,length);// start from number 1, and add length
		else
		transform(atoi(lastNum.c_str())+1,length);

		// Add your own code here to experiment with the library.
	} catch(char const* err) {
		std::cout << "The library threw an exception:\n"
		<< err << std::endl;
	}
	
	//Wait for User
	string s;
	getline(std::cin, s);

	return 0;
}
