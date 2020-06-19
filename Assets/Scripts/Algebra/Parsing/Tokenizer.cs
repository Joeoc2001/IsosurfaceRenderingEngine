using Algebra.Operations;
using Rationals;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Algebra.Parsing
{
    public class Tokenizer
    {
        private readonly TextReader reader;

        public Token Token { get; private set; }
        public Rational Number { get; private set; }
        public Variable VariableValue { get; private set; }
        public string FunctionName { get; private set; }

        private char currentChar;

        public Tokenizer(TextReader reader)
        {
            this.reader = reader;
            NextChar();
            NextToken();
        }

        void NextChar()
        {
            int c = reader.Read();
            currentChar = c < 0 ? '\0' : char.ToLower((char)c);
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(currentChar))
            {
                NextChar();
            }

            switch (currentChar)
            {
                case '\0':
                    Token = Token.EOF;
                    return;

                case '+':
                    NextChar();
                    Token = Token.Add;
                    return;

                case '-':
                    NextChar();
                    Token = Token.Subtract;
                    return;

                case '*':
                    NextChar();
                    Token = Token.Multiply;
                    return;

                case '/':
                    NextChar();
                    Token = Token.Divide;
                    return;

                case '^':
                    NextChar();
                    Token = Token.Exponent;
                    return;

                case '(':
                    NextChar();
                    Token = Token.OpenBrace;
                    return;

                case ')':
                    NextChar();
                    Token = Token.CloseBrace;
                    return;

                case ',':
                    NextChar();
                    Token = Token.Comma;
                    return;
            }

            if (char.IsDigit(currentChar))
            {
                StringBuilder stringBuilder = new StringBuilder();

                bool haveDecimalPoint = false;
                bool haveDenominator = false;

                while (char.IsDigit(currentChar)
                    || (!haveDecimalPoint && !haveDenominator && currentChar == '.')
                    || (!haveDecimalPoint && !haveDenominator && currentChar == '/'))
                {
                    stringBuilder.Append(currentChar);
                    haveDecimalPoint = haveDecimalPoint || currentChar == '.';
                    haveDenominator = haveDenominator || currentChar == '/';
                    NextChar();
                }

                if (!haveDenominator)
                {
                    Number = Rational.ParseDecimal(stringBuilder.ToString());
                }
                else
                {
                    Number = Rational.Parse(stringBuilder.ToString());
                }
                Token = Token.Decimal;
                return;
            }

            if (char.IsLetter(currentChar))
            {
                StringBuilder stringBuilder = new StringBuilder();
                while (char.IsLetter(currentChar))
                {
                    stringBuilder.Append(currentChar);
                    NextChar();
                }

                string identifier = stringBuilder.ToString();
                if (Variable.VariableDict.TryGetValue(identifier, out Variable v))
                {
                    VariableValue = v;
                    Token = Token.Variable;
                    return;
                }

                Token = Token.Function;
                FunctionName = identifier;
                return;
            }

            throw new InvalidDataException($"Unexpected token '{currentChar + reader.ReadToEnd()}'");
        }
    }
}