using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ParserTests
    {
        [Test]
        public void Parser_ParsesVariables()
        {
            foreach (string key1 in Variable.VariableDict.Keys)
            {
                // ARANGE
                string equation = key1;
                Variable expected = Variable.VariableDict[key1];

                // ACT
                Equation result = Parser.Parse(equation);

                // ASSERT
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void Parser_ParsesConstants()
        {
            // ARANGE
            string equation = "987654321.5";
            Constant expected = new Constant((Rational)987654321.5M);

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesAddition()
        {
            // ARANGE
            string equation = "x + 1";
            Equation expected = Variable.X + 1;

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesMultiplication()
        {
            // ARANGE
            string equation = "x * y";
            Equation expected = Variable.X * Variable.Y;

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesSubtraction()
        {
            // ARANGE
            string equation = "x - 50";
            Equation expected = Variable.X + (-50);

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesDivision()
        {
            // ARANGE
            string equation = "x / 2";
            Equation expected = Variable.X / 2;

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesExponentiation()
        {
            // ARANGE
            string equation = "x ^ y";
            Equation expected = Equation.Pow(Variable.X, Variable.Y);

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesBraces()
        {
            // ARANGE
            string equation = "(x + y) * 5";
            Equation expected = (Variable.X + Variable.Y) * 5;

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesSingleNegation()
        {
            // ARANGE
            string equation = "-5";
            Equation expected = Constant.MINUS_ONE * 5;

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesDoubleNegation()
        {
            // ARANGE
            string equation = "--5";
            Equation expected = Constant.MINUS_ONE * (Constant.MINUS_ONE * 5);

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesLogBraceless()
        {
            // ARANGE
            string equation = "ln 5";
            Equation expected = new LnOperation(new Constant(5));

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesLogBraces()
        {
            // ARANGE
            string equation = "log(52)";
            Equation expected = new LnOperation(new Constant(52));

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesChainedLogBraceless()
        {
            // ARANGE
            string equation = "log ln 15";
            Equation expected = new LnOperation(new LnOperation(new Constant(15)));

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parser_ParsesChainedLogBraces()
        {
            // ARANGE
            string equation = "ln(log(15))";
            Equation expected = new LnOperation(new LnOperation(new Constant(15)));

            // ACT
            Equation result = Parser.Parse(equation);

            // ASSERT
            Assert.AreEqual(expected, result);
        }
    }
}
