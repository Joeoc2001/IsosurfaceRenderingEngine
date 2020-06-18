using System;
using System.CodeDom;
using System.Collections.Generic;

public class EquivalencePath
{
    public static readonly EquivalencePath IDENTITY = new EquivalencePath(
        eq => new List<Equation> { eq }
    );
    

    public readonly Func<Equation, IEnumerable<Equation>> func;

    public EquivalencePath(Func<Equation, IEnumerable<Equation>> func)
    {
        this.func = func;
    }
}
