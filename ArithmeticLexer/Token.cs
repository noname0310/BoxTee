using System;

namespace ArithmeticLexer
{
    internal enum TokenType
    {
        Unknown,
        Eof,
        Add,
        Sub,
        Mul,
        Div,
        LiteralInteger
    }

    internal readonly struct Token
    {
        public readonly TokenType TokenType;
        public readonly ReadOnlyMemory<char> TokenContent;
        public readonly int LineNumber;
        public readonly int LineOffset;

        public Token(TokenType tokenType, ReadOnlyMemory<char> tokenContent, int lineNumber, int lineOffset)
        {
            TokenType = tokenType;
            TokenContent = tokenContent;
            LineNumber = lineNumber;
            LineOffset = lineOffset;
        }

        public override string ToString() =>
            $"(TokenType: {TokenType}, TokenContent: {TokenContent}, LineNumber: {LineNumber}, LineOffset: {LineOffset}";
    }
}
