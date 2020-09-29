using Algebra;
using AlgebraExtensions;

namespace SDFRendering
{
    public class ImplicitSurface
    {
        public readonly Expression Expression;
        public readonly Vector3Expression Gradient;

        public ImplicitSurface(Expression e)
        {
            Expression = e;
            Gradient = Vector3Expression.GetFromGradient(e);
        }

    }
}
