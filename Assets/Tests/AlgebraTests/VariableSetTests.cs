using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;
using Algebra;
using Algebra.Operations;
using Algebra.Parsing;

namespace AlgebraTests
{
    public class VariableSetTests
    {
        [Test]
        public void VariableSet_EmptyConstructor_SetsAllTo0()
        {
            // ARANGE
            VariableSet set = new VariableSet();

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                Assert.AreEqual(0f, set[v]);
            }
        }

        [Test]
        public void VariableSet_floatConstructor_SetsX()
        {
            // ARANGE
            VariableSet set = new VariableSet(29f);

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch(v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(29f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_Vector2Constructor_SetsXY()
        {
            // ARANGE
            VariableSet set = new VariableSet(new Vector2(27f, -17f));

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(27f, set[v]);
                        break;
                    case "y":
                        Assert.AreEqual(-17f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_Vector3Constructor_SetsXYZ()
        {
            // ARANGE
            VariableSet set = new VariableSet(new Vector3(-28f, 1023f, 94f));

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(-28f, set[v]);
                        break;
                    case "y":
                        Assert.AreEqual(1023f, set[v]);
                        break;
                    case "z":
                        Assert.AreEqual(94f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_floatArrayConstructor_Sets()
        {
            // ARANGE
            VariableSet set = new VariableSet(new float[] { 9204f, 1f });

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(9204f, set[v]);
                        break;
                    case "y":
                        Assert.AreEqual(1f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_floatImplicitCast_SetsX()
        {
            // ARANGE
            VariableSet set = 29f;

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(29f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_Vector2ImplicitCast_SetsXY()
        {
            // ARANGE
            VariableSet set = new Vector2(27f, -17f);

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(27f, set[v]);
                        break;
                    case "y":
                        Assert.AreEqual(-17f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_Vector3ImplicitCast_SetsXYZ()
        {
            // ARANGE
            VariableSet set = new Vector3(-28f, 1023f, 94f);

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(-28f, set[v]);
                        break;
                    case "y":
                        Assert.AreEqual(1023f, set[v]);
                        break;
                    case "z":
                        Assert.AreEqual(94f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_floatArrayImplicitCast_Sets()
        {
            // ARANGE
            VariableSet set = new float[] { 92304f, 1f};

            // ACT

            // ASSERT
            foreach (Variable v in Variable.VariableDict.Values)
            {
                switch (v.Name.ToLower())
                {
                    case "x":
                        Assert.AreEqual(92304f, set[v]);
                        break;
                    case "y":
                        Assert.AreEqual(1f, set[v]);
                        break;
                    default:
                        Assert.AreEqual(0f, set[v]);
                        break;
                }
            }
        }

        [Test]
        public void VariableSet_EmptySets_AreEqual()
        {
            // ARANGE

            // ACT
            VariableSet set1 = new VariableSet();
            VariableSet set2 = new VariableSet();

            // ASSERT
            Assert.IsTrue(set1.Equals(set2));
            Assert.IsTrue(set2.Equals(set1));
            Assert.IsTrue(set1.Equals((object)set2));
            Assert.IsTrue(set2.Equals((object)set1));
            Assert.IsTrue(set1 == set2);
            Assert.IsTrue(set2 == set1);
            Assert.IsFalse(set1 != set2);
            Assert.IsFalse(set2 != set1);
        }

        [Test]
        public void VariableSet_DifferentSets_AreDifferent()
        {
            // ARANGE

            // ACT
            VariableSet set1 = new VariableSet(new Vector3(5, 2, 1));
            VariableSet set2 = new VariableSet(new Vector3(5, 2, 1.00001f));

            // ASSERT
            Assert.IsFalse(set1.Equals(set2));
            Assert.IsFalse(set2.Equals(set1));
            Assert.IsFalse(set1.Equals((object)set2));
            Assert.IsFalse(set2.Equals((object)set1));
            Assert.IsFalse(set1 == set2);
            Assert.IsFalse(set2 == set1);
            Assert.IsTrue(set1 != set2);
            Assert.IsTrue(set2 != set1);
        }
    }
}
