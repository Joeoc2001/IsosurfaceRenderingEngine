using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EquationMapping
{
    public static implicit operator EquationMapping(EquationMap map) => new EquationMapping() { PostMap = map };

    public delegate Equation EquationMap(Equation a);
    public delegate bool EquationFilter(Equation a);

    public EquationMap PostMap = (a => a);
    public EquationFilter ShouldMapChildren = (a => true);
    public EquationFilter ShouldMapThis = (a => true);
}
