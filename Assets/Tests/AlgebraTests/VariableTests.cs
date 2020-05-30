using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class VariableTests
    {
        [Test]
        public void Variable_IsSelfEqual()
        {
            foreach (string key in Variable.VariableDict.Keys)
            {
                // ARANGE
                Variable v1 = Variable.VariableDict[key];
                Variable v2 = Variable.VariableDict[key];

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
        }

        [Test]
        public void Variable_IsOtherNotEqual()
        {
            foreach (string key1 in Variable.VariableDict.Keys)
            {
                // ARANGE
                Variable v1 = Variable.VariableDict[key1];
                foreach (string key2 in Variable.VariableDict.Keys)
                {
                    if (key2.Equals(key1))
                    {
                        continue;
                    }
                    Variable v2 = Variable.VariableDict[key2];

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
            }
        }

        [Test]
        public void Variable_Derivative_IsOne_WRTSelf()
        {
            foreach (string key in Variable.VariableDict.Keys)
            {
                // ARANGE
                Variable v1 = Variable.VariableDict[key];

                // ACT
                Equation derivative = v1.GetDerivative(v1);

                // ASSERT
                Assert.AreEqual(Constant.From(1), derivative);
            }
        }

        [Test]
        public void Variable_Derivative_IsZero_WRTOthers()
        {
            foreach (string key1 in Variable.VariableDict.Keys)
            {
                // ARANGE
                Variable v1 = Variable.VariableDict[key1];
                foreach (string key2 in Variable.VariableDict.Keys)
                {
                    if (key2.Equals(key1))
                    {
                        continue;
                    }
                    Variable v2 = Variable.VariableDict[key2];

                    // ACT
                    Equation derivative = v1.GetDerivative(v2);

                    // ASSERT
                    Assert.AreEqual(Constant.From(0), derivative);
                }
            }
        }

        [Test]
        public void Variable_EvaluatesCorrectly()
        {
            List<string> keys = new List<string>(Variable.VariableDict.Keys);

            float[] values = new float[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                values[i] = keys.Count * i;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                // ARANGE
                string key = keys[i];
                Variable v = Variable.VariableDict[key];

                // ACT
                float value = v.GetExpression()(new VariableSet(values));

                // ASSERT
                Assert.AreEqual(values[i], value);
            }
        }
    }
}
