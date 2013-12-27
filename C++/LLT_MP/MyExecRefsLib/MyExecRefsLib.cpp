// Sample program demonstrating the use of the Big Integer Library.

// Standard libraries
#include <string>
#include <iostream>
#include <sstream>
// `BigIntegerLibrary.hh' includes all of the library headers.
#include "BigIntegerLibrary.hh"

int main() {
	/* The library throws `const char *' error messages when things go
	 * wrong.  It's a good idea to catch them using a `try' block like this
	 * one.  Your C++ compiler might need a command-line option to compile
	 * code that uses exceptions. */
	try {
		// Instead you can convert the number from a string.
		std::string s("1234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678");
		BigInteger a = stringToBigInteger(s);
		BigInteger b = stringToBigInteger(s);
		BigInteger c = a*b;
		// f is implicitly stringified and sent to std::cout.
		std::cout << "a\n" << a<<std::endl;
		std::cout << "b\n" << b<<std::endl;
		std::cout << "c\n" << c<<std::endl;
		
		// Let's do some heavy lifting and calculate powers of 314.
		int maxPower = 10;
		BigUnsigned x(1), big314(314);
		for (int power = 0; power <= maxPower; power++) {
			std::cout << "314^" << power << " = " << x << std::endl;
			x *= big314; // A BigInteger assignment operator
		}

		// Add your own code here to experiment with the library.
	} catch(char const* err) {
		std::cout << "The library threw an exception:\n"
			<< err << std::endl;
	}
	std::string s;
	std::getline(std::cin, s);
	return 0;
}

/*
The original sample program produces this output:

3141592653589793238462643383279
314424
313894
83252135
1185
134
0xFF
0xFF00FFFF
0xFF00FF00
0x1FFFE00000
0x3F
314^0 = 1
314^1 = 314
314^2 = 98596
314^3 = 30959144
314^4 = 9721171216
314^5 = 3052447761824
314^6 = 958468597212736
314^7 = 300959139524799104
314^8 = 94501169810786918656
314^9 = 29673367320587092457984
314^10 = 9317437338664347031806976
12
8
1931

*/
