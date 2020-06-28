using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;
using Algebra;
using Algebra.Operations;
using Algebra.Parsing;

namespace OperationsTests
{
    public class SinTests
    {
        [Test]
        public void Sin_IsEqual_WhenSame()
        {
            // ARANGE
            Equation v1 = Equation.SinOf(Variable.X);
            Equation v2 = Equation.SinOf(Variable.X);

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
        public void Sin_EqualReturnFalse_WhenDifferent()
        {
            // ARANGE
            Equation v1 = Equation.SinOf(Variable.X);
            Equation v2 = Equation.SinOf(Variable.Y);

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
        public void Sin_XDerivative_IsCorrect()
        {
            // ARANGE
            Equation value = Equation.SinOf(Variable.X);
            Equation expected = Equation.CosOf(Variable.X);

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Sin_XSquaredDerivative_IsCorrect()
        {
            // ARANGE
            Equation value = Equation.SinOf(Equation.Pow(Variable.X, 2));
            Equation expected = 2 * Variable.X * Equation.CosOf(Equation.Pow(Variable.X, 2));

            // ACT
            Equation derivative = value.GetDerivative(Variable.X);

            // ASSERT
            Assert.AreEqual(expected, derivative);
        }

        [Test]
        public void Sin_Evaluates0Correctly()
        {
            // ARANGE
            Equation equation = Equation.SinOf(0);

            // ACT
            float value = equation.GetExpression()(new VariableSet());

            // ASSERT
            Assert.AreEqual(0, value);
        }

        [Test]
        public void Sin_DoesntSimplify_WhenConstantParameter()
        {
            // ARANGE

            // ACT
            Equation e = Equation.SinOf(10);

            // ASSERT
            Assert.IsFalse(e is Constant);
        }

        [Test]
        public void Sin_GetOrderIndex_Is0()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.SinOf(Variable.X);

            // ASSERT
            Assert.AreEqual(0, equation.GetOrderIndex());
        }

        [Test]
        public void Sin_GetHash_IsNotArgumentHash1()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.Y;
            Equation equation = Equation.SinOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sin_GetHash_IsNotArgumentHash2()
        {
            // ARANGE

            // ACT
            Equation argument = 2;
            Equation equation = Equation.SinOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sin_GetHash_IsNotArgumentHash3()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.X + 1;
            Equation equation = Equation.SinOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sin_GetHash_IsNotSignHash()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.X;
            Equation ln = Equation.SinOf(argument);
            Equation sign = Equation.SignOf(argument);
            int hash1 = ln.GetHashCode();
            int hash2 = sign.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sin_Map_DoesntChangeOriginal()
        {
            // ARANGE
            Equation equation1 = Equation.SinOf(Variable.X);
            Equation equation2 = Equation.SinOf(Variable.X);

            // ACT
            equation2.Map(a => Equation.SinOf(Variable.Y));

            // ASSERT
            Assert.AreEqual(equation1, equation2);
        }

        [Test]
        public void Sin_Map_ReturnsAlternative()
        {
            // ARANGE
            Equation equation1 = Equation.SinOf(Variable.X);

            // ACT
            Equation equation2 = equation1.Map(a => Equation.SinOf(Variable.Y));

            // ASSERT
            Assert.AreEqual(Equation.SinOf(Variable.Y), equation2);
        }

        [Test]
        public void Sin_Map_MapsChildren()
        {
            // ARANGE
            Equation equation1 = Equation.SinOf(Variable.X);

            // ACT
            Equation equation2 = equation1.Map(a => a is Variable ? Variable.Z : a);

            // ASSERT
            Assert.AreEqual(Equation.SinOf(Variable.Z), equation2);
        }

        [Test]
        public void Sin_Map_CanSkipSelf()
        {
            // ARANGE
            Equation equation1 = Equation.SinOf(Variable.X);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => Variable.Z,
                ShouldMapThis = a => !(a is Sin)
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.SinOf(Variable.Z), equation2);
        }

        [Test]
        public void Sin_Map_CanSkipChildren()
        {
            // ARANGE
            Equation equation1 = Equation.SinOf(Variable.X);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => a is Variable ? Variable.Z : a,
                ShouldMapChildren = a => false
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.SinOf(Variable.X), equation2);
        }
    }
}
