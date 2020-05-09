using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AdditionTests
    {
        [Test]
        public void Addition_IsEqual_WhenSame()
        {
            // ARANGE
            Equation v1 = Variable.X + 1;
            Equation v2 = Variable.X + 1;

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
        public void Addition_IsEqual_Commutative()
        {
            // ARANGE
            Equation v1 = Variable.X + 1;
            Equation v2 = 1 + Variable.X;

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
        public void Addition_EqualReturnFalse_WhenDifferent()
        {
            // ARANGE
            Equation v1 = Variable.X + 1;
            Equation v2 = Variable.X + 2;

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
        public void Addition_Derivative_IsDerivativeSum()
        {
            // ARANGE
            Equation value = Variable.X + Variable.Y;
            Equation expected = Constant.ONE + Constant.ZERO;

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Addition_EvaluatesCorrectly()
        {
            // ARANGE
            Equation equation = new Constant(54321) + new Constant(7);

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(54321 + 7, value);
        }

        [Test]
        public void Addition_Simplify_CollectsConstants()
        {
            // ARANGE
            Equation equation = new Constant(54321) + Variable.Z + new Constant(54321);
            Equation expected = new Constant(54321 + 54321) + Variable.Z;

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Addition_Simplify_CollectsCoefficients()
        {
            // ARANGE
            Equation equation = (new Constant(54321) * Variable.Z) + (new Constant(54321) * Variable.Z) + Variable.Z;
            Equation expected = new Constant(54321 + 54321 + 1) * Variable.Z;

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Addition_Simplify_RemovesZeros()
        {
            // ARANGE
            Equation equation = Constant.ZERO + Constant.ONE + Constant.MINUS_ONE + Variable.Z;
            Equation expected = Variable.Z;

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }

        [Test]
        public void Addition_Simplify_CollatesAdditions()
        {
            // ARANGE
            Equation equation = new Addition(new Equation[] { new Addition(new Equation[] { Variable.X, Variable.Y } ), Variable.Z });
            Equation expected = new Addition(new Equation[] { Variable.X, Variable.Y, Variable.Z });

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(expected, simplified);
        }
    }
}
