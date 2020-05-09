using System;
using System.Collections.Generic;

public class Variable : Equation, IEquatable<Variable>
{
    public static readonly Variable X = new Variable(0, "x");
    public static readonly Variable Y = new Variable(1, "y");
    public static readonly Variable Z = new Variable(2, "z");

    public static readonly Dictionary<string, Variable> VariableDict = new Dictionary<string, Variable>() 
    {
        { X.Name, X },
        { Y.Name, Y },
        { Z.Name, Z }
    };

    public readonly int Index;
    public readonly string Name;

    private Variable(int index, string name)
    {
        this.Index = index;
        this.Name = name;
    }

    public override Func<VariableSet, float> GetExpression()
    {
        return v => v[this];
    }

    public override Equation GetDerivative(Variable wrt)
    {
        if (wrt == this)
        {
            return Constant.ONE;
        }
        return Constant.ZERO;
    }

    public bool Equals(Variable obj)
    {
        if (obj is null)
        {
            return false;
        }

        return this.Index.Equals(obj.Index);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Variable);
    }

    public override int GetHashCode()
    {
        return Index;
    }

    public override string ToString()
    {
        return Name;
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
