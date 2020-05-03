using System.Numerics;

namespace Rationals
{
    /// <summary>
    /// Rational number.
    /// </summary>
    public partial struct Rational
    {
        /// <summary>
        /// Checks if the number can be simplified.
        /// </summary>
        public bool IsCanonical
        {
            get
            {
                if (Numerator.IsZero)
                {
                    return Denominator.IsOne;
                }

                var gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
                if (gcd.Sign < 0)
                    gcd = BigInteger.Negate(gcd);
                return gcd.IsOne;
            }
        }

        /// <summary>
        /// Cannonical form is irreducible fraction where denominator is always positive.
        /// </summary>
        /// <remarks>Canonical form of zero is 0/1.</remarks>
        public Rational CanonicalForm
        {
            get
            {
                if (Numerator.IsZero)
                    return Zero;

                var gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);

                if (Denominator.Sign < 0)
                    gcd = BigInteger.Negate(gcd);
                // ensures that canonical form is either positive or has minus in numerator

                var canonical = new Rational(Numerator / gcd, Denominator / gcd);
                return canonical;
            }
        }
    }
}