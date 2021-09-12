using System;
using ArithmeticLexer;

Console.WriteLine("Hello World!");

using var lexer = new Lexer(@"1+2  3  22 3
45435 + 2231 - 123231");