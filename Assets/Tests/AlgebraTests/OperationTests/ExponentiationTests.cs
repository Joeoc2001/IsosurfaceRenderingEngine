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
            Equation expected = Equation.LnOf(5) * Equation.Pow(5, Variable.X);

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
            Equation expected = (1 + Equation.LnOf(Variable.X)) * Equation.Pow(Variable.X, Variable.X);

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

        [Test]
        public void Exponentiation_GetOrderIndex_Is0()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.Pow(Variable.Y, 3);

            // ASSERT
            Assert.AreEqual(10, equation.GetOrderIndex());
        }

        [Test]
        public void Exponentiation_GetHash_IsNotCommutative1()
        {
            // ARANGE

            // ACT
            Equation equation1 = Equation.Pow(Variable.Y, 3);
            Equation equation2 = Equation.Pow(3, Variable.Y);
            int hash1 = equation1.GetHashCode();
            int hash2 = equation2.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Exponentiation_GetHash_IsNotCommutative2()
        {
            // ARANGE

            // ACT
            Equation equation1 = Equation.Pow(Variable.Y, Variable.X);
            Equation equation2 = Equation.Pow(Variable.X, Variable.Y);
            int hash1 = equation1.GetHashCode();
            int hash2 = equation2.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Exponentiation_GetHash_IsNotCommutative3()
        {
            // ARANGE

            // ACT
            Equation equation1 = Equation.Pow(Variable.Y, Variable.X + 1);
            Equation equation2 = Equation.Pow(Variable.X + 1, Variable.Y);
            int hash1 = equation1.GetHashCode();
            int hash2 = equation2.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Exponentiation_Map_DoesntChangeOriginal()
        {
            // ARANGE
            Equation equation1 = Equation.Pow(Variable.X, 2);
            Equation equation2 = Equation.Pow(Variable.X, 2);

            // ACT
            equation2.Map(a => Equation.Pow(Variable.Y, 4));

            // ASSERT
            Assert.AreEqual(equation1, equation2);
        }

        [Test]
        public void Exponentiation_Map_ReturnsAlternative()
        {
            // ARANGE
            Equation equation1 = Equation.Pow(Variable.X, 2);

            // ACT
            Equation equation2 = equation1.Map(a => Equation.Pow(Variable.Y, 4));

            // ASSERT
            Assert.AreEqual(Equation.Pow(Variable.Y, 4), equation2);
        }

        [Test]
        public void Exponentiation_Map_MapsChildren()
        {
            // ARANGE
            Equation equation1 = Equation.Pow(Variable.X, 5);

            // ACT
            Equation equation2 = equation1.Map(a => a is Variable ? Variable.Z : a);

            // ASSERT
            Assert.AreEqual(Equation.Pow(Variable.Z, 5), equation2);
        }

        [Test]
        public void Exponentiation_Map_CanSkipSelf()
        {
            // ARANGE
            Equation equation1 = Equation.Pow(Variable.X, Variable.Y);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => Variable.Z,
                ShouldMapThis = a => !(a is Exponent)
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.Pow(Variable.Z, Variable.Z), equation2);
        }

        [Test]
        public void Exponentiation_Map_CanSkipChildren()
        {
            // ARANGE
            Equation equation1 = Equation.Pow(Variable.X, 5);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => a is Variable ? Variable.Z : a,
                ShouldMapChildren = a => false
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.Pow(Variable.X, 5), equation2);
        }
    }
}
