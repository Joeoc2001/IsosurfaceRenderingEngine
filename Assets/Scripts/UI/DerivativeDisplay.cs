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
        stringBuilder.Append(e.GetDerivative(Variable.X).ToParsableString());
        stringBuilder.Append("\ndf/dy = ");
        stringBuilder.Append(e.GetDerivative(Variable.Y).ToParsableString());
        stringBuilder.Append("\ndf/dz = ");
        stringBuilder.Append(e.GetDerivative(Variable.Z).ToParsableString());

        text.text = stringBuilder.ToString();
    }

    private void Start()
    {
        text = GetComponent<Text>();
        provider.AddListener(x => GenerateDerivativeDisplay());
        GenerateDerivativeDisplay();
    }
}
