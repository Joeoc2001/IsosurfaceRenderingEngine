using System;

abstract class Monad : Equation, IEquatable<Monad>
{
    public readonly Equation Argument;

    protected Monad(Equation argument)
    {
        this.Argument = argument;
    }

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
}
