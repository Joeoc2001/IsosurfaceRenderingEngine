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
    public class LogTests
    {
        [Test]
        public void Log_DoesntSimplify_WhenConstantParameter()
        {
            // ARANGE

            // ACT
            Equation e = Equation.LnOf(10);

            // ASSERT
            Assert.IsFalse(e is Constant);
        }

        [Test]
        public void Log_GetOrderIndex_Is0()
        {
            // ARANGE

            // ACT
            Equation equation = Equation.LnOf(Variable.X);

            // ASSERT
            Assert.AreEqual(0, equation.GetOrderIndex());
        }

        [Test]
        public void Log_GetHash_IsNotArgumentHash1()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.Y;
            Equation equation = Equation.LnOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Log_GetHash_IsNotArgumentHash2()
        {
            // ARANGE

            // ACT
            Equation argument = 2;
            Equation equation = Equation.LnOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Log_GetHash_IsNotArgumentHash3()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.X + 1;
            Equation equation = Equation.LnOf(argument);
            int hash1 = argument.GetHashCode();
            int hash2 = equation.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Log_GetHash_IsNotSignHash()
        {
            // ARANGE

            // ACT
            Equation argument = Variable.X;
            Equation ln = Equation.LnOf(argument);
            Equation sign = Equation.SignOf(argument);
            int hash1 = ln.GetHashCode();
            int hash2 = sign.GetHashCode();

            // ASSERT
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void Log_Map_DoesntChangeOriginal()
        {
            // ARANGE
            Equation equation1 = Equation.LnOf(Variable.X);
            Equation equation2 = Equation.LnOf(Variable.X);

            // ACT
            equation2.Map(a => Equation.LnOf(Variable.Y));

            // ASSERT
            Assert.AreEqual(equation1, equation2);
        }

        [Test]
        public void Log_Map_ReturnsAlternative()
        {
            // ARANGE
            Equation equation1 = Equation.LnOf(Variable.X);

            // ACT
            Equation equation2 = equation1.Map(a => Equation.LnOf(Variable.Y));

            // ASSERT
            Assert.AreEqual(Equation.LnOf(Variable.Y), equation2);
        }

        [Test]
        public void Log_Map_MapsChildren()
        {
            // ARANGE
            Equation equation1 = Equation.LnOf(Variable.X);

            // ACT
            Equation equation2 = equation1.Map(a => a is Variable ? Variable.Z : a);

            // ASSERT
            Assert.AreEqual(Equation.LnOf(Variable.Z), equation2);
        }

        [Test]
        public void Log_Map_CanSkipSelf()
        {
            // ARANGE
            Equation equation1 = Equation.LnOf(Variable.X);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => Variable.Z,
                ShouldMapThis = a => !(a is Ln)
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.LnOf(Variable.Z), equation2);
        }

        [Test]
        public void Log_Map_CanSkipChildren()
        {
            // ARANGE
            Equation equation1 = Equation.LnOf(Variable.X);
            EquationMapping mapping = new EquationMapping()
            {
                PostMap = a => a is Variable ? Variable.Z : a,
                ShouldMapChildren = a => false
            };

            // ACT
            Equation equation2 = equation1.Map(mapping);

            // ASSERT
            Assert.AreEqual(Equation.LnOf(Variable.X), equation2);
        }
    }
}
