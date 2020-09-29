using Algebra;
using AlgebraExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DerivativeDisplay : MonoBehaviour
{
    public ExpressionProvider provider;

    private Text _text;

    private void GenerateDerivativeDisplay()
    {
        Expression e = provider.GetExpression();

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("df/dx = ");
        stringBuilder.Append(e.GetDerivative("x").ToString());
        stringBuilder.Append("\ndf/dy = ");
        stringBuilder.Append(e.GetDerivative("y").ToString());
        stringBuilder.Append("\ndf/dz = ");
        stringBuilder.Append(e.GetDerivative("z").ToString());

        _text.text = stringBuilder.ToString();
    }

    private void Start()
    {
        _text = GetComponent<Text>();
        provider.OnExpressionChange += (p, e) => GenerateDerivativeDisplay();
        GenerateDerivativeDisplay();
    }
}
