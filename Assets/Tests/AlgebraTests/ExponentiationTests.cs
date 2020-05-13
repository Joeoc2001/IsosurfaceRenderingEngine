using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ExponentiationTests
    {
        [Test]
        public void Exponentiation_IsEqual_WhenSame()
        {
            // ARANGE
            Equation v1 = Equation.Pow(Variable.X, 2);
            Equation v2 = Equation.Pow(Variable.X, 2);

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
        public void Exponentiation_EqualReturnFalse_WhenDifferent()
        {
            // ARANGE
            Equation v1 = Equation.Pow(Variable.X, 6);
            Equation v2 = Equation.Pow(6, Variable.X);

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
        public void Exponentiation_1stPowerDerivative_IsCorrect()
        {
            // ARANGE
            Equation value = Equation.Pow(Variable.X, 1);
            Equation expected = 1;

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Exponentiation_ConstantPowerDerivative_IsCorrect()
        {
            // ARANGE
            Equation value = Equation.Pow(Variable.X, 5);
            Equation expected = 5 * Equation.Pow(Variable.X, 4);

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Exponentiation_ConstantBaseDerivative_IsCorrect()
        {
            // ARANGE
            Equation value = Equation.Pow(5, Variable.X);
            Equation expected = Rational.Log(5) * Equation.Pow(5, Variable.X);

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Exponentiation_BothVariableDerivative_IsCorrect()
        {
            // ARANGE
            Equation value = Equation.Pow(Variable.X, Variable.X);
            Equation expected = (1 + Equation.Ln(Variable.X)) * Equation.Pow(Variable.X, Variable.X);

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Exponentiation_EvaluatesCorrectly()
        {
            // ARANGE
            Equation equation = Equation.Pow(2, Constant.From(7));

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(128.0f, value);
        }

        [Test]
        public void Exponentiation_Simplify_CollapsesConstants()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Pow(5, Constant.From(2));
            Equation expected = Constant.From(25);

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Exponentiation_Simplify_RemovesPowersOf1()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Pow(Variable.Z, 1);
            Equation expected = Variable.Z;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Exponentiation_Simplify_RemovesPowersOf0()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Pow(Variable.Y, 0);
            Equation expected = 1;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }
    }
}
