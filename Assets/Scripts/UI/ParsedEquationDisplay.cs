using AlgebraExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class ParsedExpressionDisplay : MonoBehaviour
{
    public ExpressionProvider provider;

    private TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
        provider.OnExpressionChange += (p, e) => text.text = $"f(x,y,z) = {e}";
    }

    private void Start()
    {
    }
}
