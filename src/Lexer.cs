using System;
using System.Collections.Generic;
using System.Text;

namespace StarByte.Compiler
{
    public class Lexer
    {
        private string _source;
        private int _position;

        public Lexer(string source)
        {
            _source = source;
            _position = 0;
        }

        public Token NextToken()
        {
            while (_position < _source.Length)
            {
                char currentChar = _source[_position];

                if (char.IsWhiteSpace(currentChar))
                {
                    _position++;
                    continue;
                }

                if (char.IsLetter(currentChar))
                {
                    return ReadKeywordOrIdentifier();
                }

                if (char.IsDigit(currentChar))
                {
                    return ReadNumber();
                }

                if (currentChar == '=')
                {
                    _position++;
                    return new Token(TokenType.Assignment, "=");
                }

                if (currentChar == '{')
                {
                    _position++;
                    return new Token(TokenType.LBrace, "{");
                }

                if (currentChar == '}')
                {
                    _position++;
                    return new Token(TokenType.RBrace, "}");
                }

                if (currentChar == '(')
                {
                    _position++;
                    return new Token(TokenType.LeftParen, "(");
                }

                if (currentChar == ')')
                {
                    _position++;
                    return new Token(TokenType.RightParen, ")");
                }

                if (currentChar == ';')
                {
                    _position++;
                    return new Token(TokenType.Semicolon, ";");
                }

                if (currentChar == '"')
                {
                    _position++;
                    return new Token(TokenType.Quotation, "\"");
                }

                throw new Exception($"Unrecognized character: {currentChar}");
            }

            return new Token(TokenType.EndOfFile, string.Empty);
        }

        private Token ReadKeywordOrIdentifier()
        {
            StringBuilder sb = new StringBuilder();

            while (_position < _source.Length && char.IsLetterOrDigit(_source[_position]))
            {
                sb.Append(_source[_position]);
                _position++;
            }

            string value = sb.ToString();

            if (value == "int" || value == "float" || value == "string" || value == "if" || value == "exit")
            {
                return new Token(TokenType.Keyword, value);
            }

            return new Token(TokenType.Identifier, value);
        }

        private Token ReadNumber()
        {
            StringBuilder sb = new StringBuilder();

            while (_position < _source.Length && char.IsDigit(_source[_position]))
            {
                sb.Append(_source[_position]);
                _position++;
            }

            return new Token(TokenType.Int, sb.ToString());
        }
    }
}
