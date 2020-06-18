using System;

public abstract class Monad : Equation, IEquatable<Monad>
{
    public readonly Equation Argument;

    protected Monad(Equation argument)
    {
        this.Argument = argument;
    }
    public abstract Func<Equation, Equation> GetSimplifyingConstructor();

    public bool Equals(Monad obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (this.GetType() != obj.GetType())
        {
            return false;
        }

        return Argument.Equals(obj.Argument);
    }

    public override int GetHashCode()
    {
        return Argument.GetHashCode() ^ -326072314;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Monad);
    }

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
