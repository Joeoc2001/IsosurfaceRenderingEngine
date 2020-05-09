using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ConstantTests
    {
        [Test]
        public void Constant_Zero_IsSelfEqual()
        {
            // ARANGE
            Constant zero1 = Constant.ZERO;
            Constant zero2 = Constant.ZERO;

            // ACT

            // ASSERT
            Assert.IsTrue(zero1.Equals(zero2));
            Assert.IsTrue(zero2.Equals(zero1));
            Assert.IsTrue(zero1.Equals((object)zero2));
            Assert.IsTrue(zero2.Equals((object)zero1));
            Assert.IsTrue(zero1 == zero2);
            Assert.IsTrue(zero2 == zero1);
            Assert.IsFalse(zero1 != zero2);
            Assert.IsFalse(zero2 != zero1);
        }

        [Test]
        public void Constant_ZeroAndOne_AreNotEqual()
        {
            // ARANGE
            Constant zero = Constant.ZERO;
            Constant one = Constant.ONE;

            // ACT

            // ASSERT
            Assert.IsFalse(zero.Equals(one));
            Assert.IsFalse(one.Equals(zero));
            Assert.IsFalse(zero.Equals((object)one));
            Assert.IsFalse(one.Equals((object)zero));
            Assert.IsFalse(zero == one);
            Assert.IsFalse(one == zero);
            Assert.IsTrue(zero != one);
            Assert.IsTrue(one != zero);
        }

        [Test]
        public void Constant_Constants_HaveCorrectValues()
        {
            // ARANGE
            Constant zeroConst = Constant.ZERO;
            Constant oneConst = Constant.ONE;
            Constant zero = new Constant(0);
            Constant one = new Constant(1);

            // ACT

            // ASSERT
            Assert.IsTrue(zeroConst == zero);
            Assert.IsTrue(oneConst == one);
        }

        [Test]
        public void Constant_Derivative_IsZero()
        {
            // ARANGE
            Constant value = new Constant(54321);

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(Constant.ZERO, derivative);
        }

        [Test]
        public void Constant_EvaluatesCorrectly()
        {
            // ARANGE
            Constant equation = new Constant(54321);

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(54321, value);
        }

        [Test]
        public void Constant_DoesntSimplify()
        {
            // ARANGE
            Constant equation = new Constant(54321);

            // ACT
            Equation simplified = equation.GetSimplified();

            // ASSERT
            Assert.AreEqual(equation, simplified);
            Assert.IsTrue(simplified is Constant);
            Assert.AreEqual((Rational)54321, ((Constant)simplified).GetValue());
        }
    }
}
