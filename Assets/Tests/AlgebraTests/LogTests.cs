using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
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
    }
}
