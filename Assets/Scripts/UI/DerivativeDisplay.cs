using Algebra;
using Algebra.Operations;
using AlgebraExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DerivativeDisplay : MonoBehaviour
{
    public EquationProvider provider;

    private Text text;

    private void GenerateDerivativeDisplay()
    {
        Equation e = provider.GetEquation();

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("df/dx = ");
        stringBuilder.Append(e.GetDerivative(Variable.X).ToString());
        stringBuilder.Append("\ndf/dy = ");
        stringBuilder.Append(e.GetDerivative(Variable.Y).ToString());
        stringBuilder.Append("\ndf/dz = ");
        stringBuilder.Append(e.GetDerivative(Variable.Z).ToString());

        text.text = stringBuilder.ToString();
    }

    private void Start()
    {
        text = GetComponent<Text>();
        provider.OnEquationChange += (p, e) => GenerateDerivativeDisplay();
        GenerateDerivativeDisplay();
    }
}
