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
        public void Log_Simplifies_WhenConstantParameter()
        {
            // ARANGE

            // ACT
            Equation e = Equation.Ln(10);

            // ASSERT
            Assert.IsTrue(e is Constant);
            Constant c = (Constant)e;
            Assert.IsTrue(Mathf.Approximately((float)c.GetValue(), Mathf.Log(10)));
        }

        [Test]
        public void Log_DoesntSimplify_WhenNaNResult()
        {
            // ARANGE

            // ACT
            Equation e = Equation.Ln(-1);

            // ASSERT
            Assert.IsFalse(e is Constant);
        }
    }
}
