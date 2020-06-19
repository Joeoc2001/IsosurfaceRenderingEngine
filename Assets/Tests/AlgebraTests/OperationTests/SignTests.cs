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

        [Test]
        public void Sign_GetHash_IsNotArgumentHash1()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.Y;
            Equation equation = Equation.SignOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sign_GetHash_IsNotArgumentHash2()
        {
            // ARANGE

            // ACT
            Equation argument = 2;
            Equation equation = Equation.SignOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sign_GetHash_IsNotArgumentHash3()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.X + 1;
            Equation equation = Equation.SignOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Sign_Map_DoesntChangeOriginal()
        {
            // ARANGE
            Equation equation1 = Equation.SignOf(Variable.X);
            Equation equation2 = Equation.SignOf(Variable.X);

            // ACT
            equation2.Map(a => Equation.SignOf(Variable.Y));

            // ASSERT
            Assert.AreEqual(equation1, equation2);
        }

        [Test]
        public void Sign_Map_ReturnsAlternative()
        {
            // ARANGE
            Equation equation1 = Equation.SignOf(Variable.X);

            // ACT
            Equation equation2 = equation1.Map(a => Equation.SignOf(Variable.Y));

            // ASSERT
            Assert.AreEqual(Equation.SignOf(Variable.Y), equation2);
        }

        [Test]
        public void Sign_Map_MapsChildren()
        {
            // ARANGE
            Equation equation1 = Equation.SignOf(Variable.X);

            // ACT
            Equation equation2 = equation1.Map(a => a is Variable ? Variable.Z : a);

            // ASSERT
            Assert.AreEqual(Equation.SignOf(Variable.Z), equation2);
        }

        [Test]
        public void Sign_Map_CanSkipSelf()
        {
            // ARANGE
            Equation equation1 = Equation.SignOf(Variable.X);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => Variable.Z,
                ShouldMapThis = a => !(a is Sign)
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.SignOf(Variable.Z), equation2);
        }

        [Test]
        public void Sign_Map_CanSkipChildren()
        {
            // ARANGE
            Equation equation1 = Equation.SignOf(Variable.X);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => a is Variable ? Variable.Z : a,
                ShouldMapChildren = a => false
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.SignOf(Variable.X), equation2);
        }
    }
}
