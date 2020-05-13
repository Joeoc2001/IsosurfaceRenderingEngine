using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Parser
{
    Tokenizer tokenizer;

    public Parser(Tokenizer tokenizer)
    {
        this.tokenizer = tokenizer;
    }

    public static Equation Parse(string s)
    {
        Tokenizer t = new Tokenizer(new StringReader(s));
        Parser p = new Parser(t);
        return p.Parse();
    }

    public Equation Parse()
    {
        var expr = ParseAddSubtract();

        // Check everything was consumed
        if (tokenizer.Token != Token.EOF)
        {
            throw new SyntaxException("Unexpected characters at end of expression");
        }

        return expr;
    }

    // Parse an sequence of add/subtract operators
    Equation ParseAddSubtract()
    {
        // Collate all terms into a list
        List<Equation> terms = new List<Equation>
        {
            ParseMultiplyDivide()
        };

        bool subtractNext;
        while (true)
        {
            if (tokenizer.Token == Token.Add)
            {
                subtractNext = false;
            }
            else if (tokenizer.Token == Token.Subtract)
            {
                subtractNext = true;
            }
            else
            {
                return Addition.Add(terms);
            }

            // Skip the operator
            tokenizer.NextToken();

            // Parse the next term in the expression
            Equation rhs = ParseMultiplyDivide();
            if (subtractNext)
            {
                if (rhs is Constant constant)
                {
                    rhs = Constant.From(-constant.GetValue());
                }
                else
                {
                    rhs *= -1;
                }
            }
            terms.Add(rhs);
        }
    }

    // Parse an sequence of multiply/divide operators
    Equation ParseMultiplyDivide()
    {
        // Collate all terms into a list
        List<Equation> terms = new List<Equation>
        {
            ParseExponent()
        };

        bool reciprocalNext;
        while (true)
        {
            if (tokenizer.Token == Token.Multiply)
            {
                reciprocalNext = false;
            }
            else if (tokenizer.Token == Token.Divide)
            {
                reciprocalNext = true;
            }
            else
            {
                return Multiplication.Multiply(terms);
            }

            // Skip the operator
            tokenizer.NextToken();

            // Parse the next term in the expression
            Equation rhs = ParseExponent();
            if (reciprocalNext)
            {
                rhs = Equation.Pow(rhs, -1);
            }
            terms.Add(rhs);
        }
    }

    // Parse an sequence of exponent operators
    Equation ParseExponent()
    {
        Equation lhs = ParseLeaf();

        while (true)
        {
            if (tokenizer.Token != Token.Exponent)
            {
                return lhs;
            }

            // Skip the operator
            tokenizer.NextToken();

            // Parse the next term in the expression
            Equation rhs = ParseLeaf();
            lhs = Equation.Pow(lhs, rhs);
        }
    }

    // Parse a leaf node (Variable, Constant or Function)
    Equation ParseLeaf()
    {
        if (tokenizer.Token == Token.Subtract)
        {
            tokenizer.NextToken();
            return -1 * ParseLeaf();
        }

        if (tokenizer.Token == Token.Decimal)
        {
            Equation node = Constant.From(tokenizer.Number);
            tokenizer.NextToken();
            return node;
        }

        if (tokenizer.Token == Token.Variable)
        {
            Equation node = tokenizer.VariableValue;
            tokenizer.NextToken();
            return node;
        }

        if (tokenizer.Token == Token.OpenBrace)
        {
            tokenizer.NextToken();

            Equation node = ParseAddSubtract();

            if (tokenizer.Token != Token.CloseBrace)
            {
                throw new SyntaxException("Missing close parenthesis");
            }

            tokenizer.NextToken();

            return node;
        }

        if (tokenizer.Token == Token.Function)
        {
            tokenizer.NextToken();

            Equation node = ParseLeaf();

            switch(tokenizer.FunctionName)
            {
                case "log":
                case "ln":
                    return Equation.Ln(node);
                default:
                    throw new SyntaxException($"Unknown function name: {tokenizer.FunctionName}");
            }
        }

        throw new SyntaxException($"Unexpected leaf token: {tokenizer.Token}");
    }
}
