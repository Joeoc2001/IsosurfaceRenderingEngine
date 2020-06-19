using Algebra.Equivalence;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algebra;
using Algebra.Operations;
using Algebra.Parsing;

namespace EquivalenceTests
{
    public class ExpandBracesTests
    {
        private static readonly EquivalencePath ExpandBracesPath = EquivalencePaths.EXPAND_BRACES;

        [Test]
        public void ExpandBraces_Expands_DOTS()
        {
            // ARANGE
            Equation eq = (Variable.X + 1) * (Variable.X - 1);
            List<Equation> expected = new List<Equation>()
            {
                (Variable.X * Variable.X) - 1
            };

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void ExpandBraces_Expands_Quadratic()
        {
            // ARANGE
            Equation eq = (Variable.X + 1) * (Variable.X + 2);
            List<Equation> expected = new List<Equation>()
            {
                (Variable.X * Variable.X) + 3 * Variable.X + 2
            };

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void ExpandBraces_Expands_AllThreeOfCubic()
        {
            // ARANGE
            Equation eq = (Variable.X + 1) * (Variable.X + 2) * (Variable.X + 3);
            List<Equation> expected = new List<Equation>()
            {
                ((Variable.X * Variable.X) + 3 * Variable.X + 2) * (Variable.X + 3),
                ((Variable.X * Variable.X) + 4 * Variable.X + 3) * (Variable.X + 2),
                ((Variable.X * Variable.X) + 5 * Variable.X + 6) * (Variable.X + 1)
            };

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void ExpandBraces_DoesntExpandAllOfCubicAtOnce()
        {
            // ARANGE
            Equation eq = (Variable.X + 1) * (Variable.X + 2) * (Variable.X + 3);
            Equation nonexpected = Variable.X * Variable.X * Variable.X
                + 6 * Variable.X * Variable.X
                + 11 * Variable.X
                + 6;

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.IsFalse(actual.Contains(nonexpected));
        }

        [Test]
        public void ExpandBraces_Expands_Nested()
        {
            // ARANGE
            Equation eq = ((Variable.X + 1) * (Variable.X + 2) + 1) * (Variable.X + 3);
            List<Equation> expected = new List<Equation>()
            {
                ((Variable.X * Variable.X) + 3 * Variable.X + 3) * (Variable.X + 3),
                (Variable.X + 1) * (Variable.X + 2) * Variable.X + (Variable.X + 1) * (Variable.X + 2) * 3 + Variable.X + 3
            };

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void ExpandBraces_DoesntDistribute()
        {
            // ARANGE
            Equation eq = (Variable.X + 1) * 3;

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.That(actual, Has.Count.EqualTo(0));
        }

        [Test]
        public void ExpandBraces_onAddition_DoesNothing()
        {
            // ARANGE
            Equation eq = Variable.X + 1;

            // ACT
            List<Equation> actual = ExpandBracesPath(eq);

            // ASSERT
            Assert.That(actual, Has.Count.EqualTo(0));
        }
    }
}
