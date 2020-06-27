using System;
using System.Collections.Generic;


namespace Algebra.Operations
{
    public class Variable : Equation, IEquatable<Variable>
    {
        public const int VariablesCount = 4;
        public enum Variables
        {
            X, Y, Z, W
        }
        public static readonly Variable X = new Variable((int)Variables.X, "x");
        public static readonly Variable Y = new Variable((int)Variables.Y, "y");
        public static readonly Variable Z = new Variable((int)Variables.Z, "z");
        public static readonly Variable W = new Variable((int)Variables.W, "w");

        public static readonly Dictionary<string, Variable> VariableDict = new Dictionary<string, Variable>()
    {
        { X.Name, X },
        { Y.Name, Y },
        { Z.Name, Z },
        { W.Name, W }
    };

        public readonly int Index;
        public readonly string Name;

        private Variable(int index, string name)
        {
            this.Index = index;
            this.Name = name;
        }

        public override ExpressionDelegate GetExpression()
        {
            return v => v[this];
        }

        public override Equation GetDerivative(Variable wrt)
        {
            if (wrt == this)
            {
                return 1;
            }
            return 0;
        }

        public bool Equals(Variable obj)
        {
            if (obj is null)
            {
                return false;
            }

            return this.Index.Equals(obj.Index);
        }

        public override bool Equals(Equation obj)
        {
            return this.Equals(obj as Variable);
        }

        public override int GenHashCode()
        {
            return Index * 1513357220;
        }

        public override string ToString()
        {
            return Name;
        }

        public override string ToRunnableString()
        {
            return $"Variable.{Name.ToUpper()}";
        }

        public override int GetOrderIndex()
        {
            return 0;
        }

        public override Equation Map(EquationMapping map)
        {
            if (!map.ShouldMapThis(this))
            {
                return this;
            }

            return map.PostMap(this);
        }
    }
}