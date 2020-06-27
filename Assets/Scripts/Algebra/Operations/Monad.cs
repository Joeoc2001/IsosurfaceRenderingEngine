using System;


namespace Algebra.Operations
{
    public abstract class Monad : Equation
    {
        public readonly Equation Argument;

        protected Monad(Equation argument)
        {
            this.Argument = argument;
        }
        public abstract Func<Equation, Equation> GetSimplifyingConstructor();

        public override Equation Map(EquationMapping map)
        {
            Equation currentThis = this;

            if (map.ShouldMapChildren(this))
            {
                Equation mappedArg = Argument.Map(map);
                currentThis = GetSimplifyingConstructor()(mappedArg);
            }

            if (map.ShouldMapThis(this))
            {
                currentThis = map.PostMap(currentThis);
            }

            return currentThis;
        }
    }
}