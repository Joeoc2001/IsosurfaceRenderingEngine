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
    private Expression _equation;
    private TMP_InputField _field;

    private void Awake()
    {
        _field = GetComponent<TMP_InputField>();
        _field.onEndEdit.AddListener(s => UpdateExpression(s));
    }

    private void UpdateExpression(string s)
    {
        _equation = Parser.Parse(s);
        ExpressionHasChanged(_equation);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateExpression(_field.text);
    }

    public override Expression GetExpression()
    {
        return _equation;
    }
}
