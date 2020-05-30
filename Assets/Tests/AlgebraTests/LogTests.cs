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
            Equation e = Equation.Ln(10);

            // ASSERT
            Assert.IsFalse(e is Constant);
        }
    }
}
