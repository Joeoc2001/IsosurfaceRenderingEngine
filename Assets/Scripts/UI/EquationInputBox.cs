using Algebra;
using Algebra.Parsing;
using AlgebraExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class ExpressionInputBox : ExpressionProvider
{
    private Expression equation;
    private TMP_InputField field;

    private void Awake()
    {
        field = GetComponent<TMP_InputField>();
        field.onEndEdit.AddListener(s => UpdateExpression(s));
    }

    private void UpdateExpression(string s)
    {
        equation = Parser.Parse(s);
        ExpressionHasChanged(equation);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateExpression(field.text);
    }

    public override Expression GetExpression()
    {
        return equation;
    }
}
