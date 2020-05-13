﻿using System.Collections;
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
            Equation expected = Constant.From(1) + Constant.From(0);

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Addition_EvaluatesCorrectly()
        {
            // ARANGE
            Equation equation = Constant.From(54321) + Constant.From(7);

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(54321 + 7, value);
        }

        [Test]
        public void Addition_Simplify_CollectsConstants()
        {
            // ARANGE

            // ACT
            Equation equation = Constant.From(54321) + Variable.Z + Constant.From(54321);
            Equation expected = Constant.From(54321 + 54321) + Variable.Z;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Addition_Simplify_CollectsCoefficients()
        {
            // ARANGE

            // ACT
            Equation equation = (Constant.From(54321) * Variable.Z) + (Constant.From(54321) * Variable.Z) + Variable.Z;
            Equation expected = Constant.From(54321 + 54321 + 1) * Variable.Z;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Addition_Simplify_RemovesZeros()
        {
            // ARANGE

            // ACT
            Equation equation = Constant.From(0) + Constant.From(1) + Constant.From(-1) + Variable.Z;
            Equation expected = Variable.Z;

            // ASSERT
            Assert.AreEqual(expected, equation);
        }

        [Test]
        public void Addition_Simplify_CollatesAdditions()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Add(new List<Equation>() { Equation.Add(new List<Equation>() { Variable.X, Variable.Y }), Variable.Z });
            Equation expected = Equation.Add(new List<Equation>() { Variable.X, Variable.Y, Variable.Z });

            // ASSERT
            Assert.AreEqual(expected, equation);
        }
    }
}
