using System;
using System.Collections.Generic;

public class Variable : Equation, IEquatable<Variable>
{
    public static readonly Variable X = new Variable(0, "x");
    public static readonly Variable Y = new Variable(1, "y");
    public static readonly Variable Z = new Variable(2, "z");

    public static readonly Dictionary<string, Variable> VariableDict = new Dictionary<string, Variable>() 
    {
        { X.name, X },
        { Y.name, Y },
        { Z.name, Z }
    };

    private readonly int index;
    private readonly string name;

    private Variable(int index, string name)
    {
        this.index = index;
        this.name = name;
    }

    public override Func<VectorN, float> GetExpression()
    {
        return v => v[index];
    }

    public override Equation GetDerivative(Variable wrt)
    {
        if (wrt == this)
        {
            return Constant.ONE;
        }
        return Constant.ZERO;
    }

    public override Polynomial GetTaylorApproximation(VectorN origin, int order)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Variable obj)
    {
        if (obj == null)
        {
            return false;
        }

        return this.index.Equals(obj.index);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Variable);
    }

    public override int GetHashCode()
    {
        return index.GetHashCode();
    }

    public override string ToString()
    {
        return name;
    }

    public override Equation GetSimplified()
    {
        return this;
    }

    public static bool operator ==(Variable left, Variable right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Variable left, Variable right)
    {
        return !(left == right);
    }
}
