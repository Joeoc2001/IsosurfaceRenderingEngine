using NUnit.Framework;
using Algebra;
using Algebra.Operations;
using Algebra.Parsing;

namespace OperationsTests
{
    class EquationDisplayComparerTests
    {

        [Test]
        public void Variable_XComparedToX_Is0()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = Variable.X;
            int checkInt = EquationDisplayComparer.COMPARER.Compare(a, b);

            // ASSERT
            Assert.AreEqual(0, checkInt);
        }

        [Test]
        public void Variable_YComparedToY_Is0()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Y;
            Equation b = Variable.Y;
            int checkInt = EquationDisplayComparer.COMPARER.Compare(a, b);

            // ASSERT
            Assert.AreEqual(0, checkInt);
        }

        [Test]
        public void Variable_ZComparedToZ_Is0()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Z;
            Equation b = Variable.Z;
            int checkInt = EquationDisplayComparer.COMPARER.Compare(a, b);

            // ASSERT
            Assert.AreEqual(0, checkInt);
        }

        [Test]
        public void Variable_WComparedToW_Is0()
        {
            // ARANGE

            // ACT
            Equation a = Variable.W;
            Equation b = Variable.W;
            int checkInt = EquationDisplayComparer.COMPARER.Compare(a, b);

            // ASSERT
            Assert.AreEqual(0, checkInt);
        }

        [Test]
        public void Variable_X_IsLessThanY()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = Variable.Y;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_X_IsLessThanZ()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = Variable.Z;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_X_IsLessThanW()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = Variable.W;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Y_IsLessThanZ()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Y;
            Equation b = Variable.Z;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Y_IsLessThanW()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Y;
            Equation b = Variable.W;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Z_IsLessThanW()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Z;
            Equation b = Variable.W;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_X_IsLessThan1()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_X_IsLessThanMinus1()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = -1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Y_IsGreaterThanX()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Y;
            Equation b = Variable.X;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Z_IsGreaterThanX()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Z;
            Equation b = Variable.X;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_W_IsGreaterThanX()
        {
            // ARANGE

            // ACT
            Equation a = Variable.W;
            Equation b = Variable.X;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Z_IsGreaterThanY()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Z;
            Equation b = Variable.Y;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_W_IsGreaterThanY()
        {
            // ARANGE

            // ACT
            Equation a = Variable.W;
            Equation b = Variable.Y;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_W_IsGreaterThanZ()
        {
            // ARANGE

            // ACT
            Equation a = Variable.W;
            Equation b = Variable.Z;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_1_IsGreaterThanX()
        {
            // ARANGE

            // ACT
            Equation a = 1;
            Equation b = Variable.X;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Minus1_IsGreaterThanX()
        {
            // ARANGE

            // ACT
            Equation a = -1;
            Equation b = Variable.X;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XPlusOne_IsLessThanZPlusOne()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X + 1;
            Equation b = Variable.Z + 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XPlusOne_IsLessThanXMinusOne()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X + 1;
            Equation b = Variable.X - 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_X_IsLessThanXPlusOne()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = Variable.X + 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XPlusY_IsLessThanXPlusOne()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X + Variable.Y;
            Equation b = Variable.X + 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_Y_IsLessThanXPlusOne()
        {
            // ARANGE

            // ACT
            Equation a = Variable.Y;
            Equation b = Variable.X + 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XPlusY_IsLessThanYPlusOne()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X + Variable.Y;
            Equation b = Variable.Y + 1;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XSquared_IsLessThanX()
        {
            // ARANGE

            // ACT
            Equation a = Equation.Pow(Variable.X, 2);
            Equation b = Variable.X;
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XSquaredComparedToXSquared_IsZero()
        {
            // ARANGE

            // ACT
            Equation a = Equation.Pow(Variable.X, 2);
            Equation b = Equation.Pow(Variable.X, 2);

            // ASSERT
            Assert.AreEqual(0, EquationDisplayComparer.COMPARER.Compare(a, b));
        }

        [Test]
        public void Variable_XCubed_IsLessThanXSquared()
        {
            // ARANGE

            // ACT
            Equation a = Equation.Pow(Variable.X, 3);
            Equation b = Equation.Pow(Variable.X, 2);
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) < 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XSquared_IsGreaterThanX()
        {
            // ARANGE

            // ACT
            Equation a = Variable.X;
            Equation b = Equation.Pow(Variable.X, 2);
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }

        [Test]
        public void Variable_XCubed_IsGreaterThanXSquared()
        {
            // ARANGE

            // ACT
            Equation a = Equation.Pow(Variable.X, 2);
            Equation b = Equation.Pow(Variable.X, 3);
            bool check = EquationDisplayComparer.COMPARER.Compare(a, b) > 0;

            // ASSERT
            Assert.IsTrue(check);
        }
    }
}
