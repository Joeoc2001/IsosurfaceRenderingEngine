using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class EquationInputBox : EquationProvider
{
    private Equation equation;
    private TMP_InputField field;

    private void Awake()
    {
        field = GetComponent<TMP_InputField>();
        field.onEndEdit.AddListener(s => UpdateEquation(s));
    }

    private void UpdateEquation(string s)
    {
        equation = Parser.Parse(s);
        onEquationChange.Invoke(equation);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateEquation(field.text);
    }

    public override Equation GetEquation()
    {
        return equation;
    }
}
