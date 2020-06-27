using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Algebra;
using Algebra.Operations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AlgebraTests
{
    public class EquationTests
    {

        [UnityTest]
        public IEnumerator Equation_FuzzHashCollisions()
        {
            RandomEquationGenerator gen = new RandomEquationGenerator()
            {
                baseProb = 0.3f,
                maxDepth = 3
            };

            Random.InitState(0);

            Dictionary<int, int> hashes = new Dictionary<int, int>();
            int collisions = 0;
            for (int i = 0; i < 1000; i++)
            {
                Equation newEq = gen.Next();
                int newHash = newEq.GetHashCode();

                if (hashes.ContainsKey(newHash))
                {
                    collisions += hashes[newHash];
                    hashes[newHash] += 1;
                }
                else
                {
                    hashes.Add(newHash, 1);
                }

                yield return null;
            }

            Assert.That(collisions, Is.LessThan(10));
        }
    }
}
