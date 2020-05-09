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
            Equation expected = Constant.ONE * Variable.Y + Variable.X * Constant.ZERO;

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Multiplication_EvaluatesCorrectly()
        {
            // ARANGE
            Equation equation = new Constant(54321) * new Constant(7);

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(54321 * 7, value);
        }

        [Test]
        public void Multiplication_Simplify_CollectsConstants()
        {
            // ARANGE
            Equation equation = new Constant(54321) * Variable.Z * new Constant(54321);
            Equation expected = new Constant(((Rational)54321) * 54321) * Variable.Z;

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Multiplication_Simplify_CollectsPowers()
        {
            // ARANGE
            Equation equation = Equation.Pow(Variable.Z, 2) * Equation.Pow(Variable.Z, Variable.Y) * Variable.Z;
            Equation expected = Equation.Pow(Variable.Z, 3 + Variable.Y);

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Multiplication_Simplify_RemovesOnes()
        {
            // ARANGE
            Equation equation = Constant.ONE * 2 * Variable.Z * (Rational)0.5M;
            Equation expected = Variable.Z;

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Multiplication_Simplify_CollatesMultiplication()
        {
            // ARANGE
            Equation equation = new Multiplication(new Equation[] { new Multiplication(new Equation[] { Variable.X, Variable.Y } ), Variable.Z });
            Equation expected = new Multiplication(new Equation[] { Variable.X, Variable.Y, Variable.Z });

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Multiplication_Simplify_GoesToConstantZero()
        {
            // ARANGE
            Equation equation = Constant.ZERO * 2 * Variable.Z * (Rational)0.5M;
            Equation expected = Constant.ZERO;

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }
    }
}
