using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SignTests
    {
        [Test]
        public void Sign_Simplifies_WhenConstantParameter()
        {
            // ARANGE
            Constant c = Constant.ONE;

            // ACT
            Equation e = Equation.SignOf(10);

            // ASSERT
            Assert.IsTrue(e is Constant);
            Assert.AreEqual(c, e);
        }

        [Test]
        public void Sign_ReturnsZero_WhenZero()
        {
            // ARANGE
            Constant c = Constant.ZERO;

            // ACT
            Equation e = Equation.SignOf(0);

            // ASSERT
            Assert.IsTrue(e is Constant);
            Assert.AreEqual(c, e);
        }

        [Test]
        public void Sign_EvaluatesToZero_WhenZero()
        {
            // ARANGE

            // ACT
            float e = Equation.SignOf(Variable.X).GetExpression()(new VariableSet(0));

            // ASSERT
            Assert.AreEqual(0, e);
        }

        [Test]
        public void Sign_EvaluatesToOne_WhenPositive()
        {
            // ARANGE

            // ACT
            float e = Equation.SignOf(Variable.X).GetExpression()(new VariableSet(145));

            // ASSERT
            Assert.AreEqual(1, e);
        }

        [Test]
        public void Sign_EvaluatesToMinusOne_WhenNegative()
        {
            // ARANGE

            // ACT
            float e = Equation.SignOf(Variable.X).GetExpression()(new VariableSet(-14335));

            // ASSERT
            Assert.AreEqual(-1, e);
        }

        [Test]
        public void Sign_GetOrderIndex_Is0()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.SignOf(Variable.X);

            // ASSERT
            Assert.AreEqual(0, equation.GetOrderIndex());
        }
    }
}
