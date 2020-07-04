using Algebra;

public class SDF
{
    public readonly Equation.ExpressionDelegate Dist;
    public readonly Equation.Vector3ExpressionDelegate Grad;

    public SDF(Equation e)
    {
        Dist = e.GetExpression();
        Grad = e.GetDerivitiveExpressionWrtXYZ();
    }
}
