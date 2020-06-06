using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MultiplicationTests
    {
        [Test]
        public void Multiplication_IsEqual_WhenSame()
        {
            // ARANGE
            Equation v1 = Variable.X * 2;
            Equation v2 = Variable.X * 2;

            // ACT

            // ASSERT
            Assert.IsTrue(v1.Equals(v2));
            Assert.IsTrue(v2.Equals(v1));
            Assert.IsTrue(v1.Equals((object)v2));
            Assert.IsTrue(v2.Equals((object)v1));
            Assert.IsTrue(v1 == v2);
            Assert.IsTrue(v2 == v1);
            Assert.IsFalse(v1 != v2);
            Assert.IsFalse(v2 != v1);
        }

        [Test]
        public void Multiplication_IsEqual_Commutative()
        {
            // ARANGE
            Equation v1 = Variable.X * 7;
            Equation v2 = 7 * Variable.X;

            // ACT

            // ASSERT
            Assert.IsTrue(v1.Equals(v2));
            Assert.IsTrue(v2.Equals(v1));
            Assert.IsTrue(v1.Equals((object)v2));
            Assert.IsTrue(v2.Equals((object)v1));
            Assert.IsTrue(v1 == v2);
            Assert.IsTrue(v2 == v1);
            Assert.IsFalse(v1 != v2);
            Assert.IsFalse(v2 != v1);
        }

        [Test]
        public void Multiplication_EqualReturnFalse_WhenDifferent()
        {
            // ARANGE
            Equation v1 = Variable.X * 1;
            Equation v2 = Variable.X * 2;

            // ACT

            // ASSERT
            Assert.IsFalse(v1.Equals(v2));
            Assert.IsFalse(v2.Equals(v1));
            Assert.IsFalse(v1.Equals((object)v2));
            Assert.IsFalse(v2.Equals((object)v1));
            Assert.IsFalse(v1 == v2);
            Assert.IsFalse(v2 == v1);
            Assert.IsTrue(v1 != v2);
            Assert.IsTrue(v2 != v1);
        }

        [Test]
        public void Multiplication_Derivative_IsTermsSum()
        {
            // ARANGE
            Equation value = Variable.X * Variable.Y;
            Equation expected = 1 * Variable.Y + Variable.X * 0;

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Multiplication_EvaluatesCorrectly()
        {
            // ARANGE
            Equation equation = Constant.From(54321) * Constant.From(7);

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(54321 * 7, value);
        }

        [Test]
        public void Multiplication_Simplify_CollectsConstants()
        {
            // ARANGE

            // ACT
            Equation equation = Constant.From(54321) * Variable.Z * Constant.From(54321);
            Equation expected = Constant.From(((Rational)54321) * 54321) * Variable.Z;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_CollectsPowers()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Pow(Variable.Z, 2) * Equation.Pow(Variable.Z, Variable.Y) * Variable.Z;
            Equation expected = Equation.Pow(Variable.Z, 3 + Variable.Y);

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_RemovesOnes()
        {
            // ARANGE

            // ACT
            Equation equation = 1 * 2 * Variable.Z * (Rational)0.5M;
            Equation expected = Variable.Z;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_CollatesMultiplication()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Multiply(new List<Equation>() { Equation.Multiply(new List<Equation>() { Variable.X, Variable.Y }), Variable.Z });
            Equation expected = Equation.Multiply(new List<Equation>() { Variable.X, Variable.Y, Variable.Z });

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_GoesToConstantZero()
        {
            // ARANGE

            // ACT
            Equation equation = 0 * 2 * Variable.Z * (Rational)0.5M;
            Equation expected = 0;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_DOTS()
        {
            // ARANGE

            // ACT
            Equation equation = (Variable.X + 1) * (Variable.X - 1);
            Equation expected = Equation.Pow(Variable.X, 2) - 1;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_ExpandsBracesToConstant()
        {
            // ARANGE

            // ACT
            Equation equation = (Variable.X + 1) * (Variable.X + 2) - (Equation.Pow(Variable.X, 2) + Variable.X * 3);
            Equation expected = 2;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_Simplify_Distributes()
        {
            // ARANGE

            // ACT
            Equation equation = 3 * (Variable.X + 1) - 3;
            Equation expected = 3 * Variable.X;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Multiplication_GetOrderIndex_Is0()
        {
            // ARANGE

            // ACT
            Equation equation = 3 * Variable.X;

            // ASSERT
            Assert.AreEqual(20, equation.GetOrderIndex());
        }
    }
}
