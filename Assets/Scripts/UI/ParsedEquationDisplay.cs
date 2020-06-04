using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class ParsedEquationDisplay : MonoBehaviour
{
    public EquationProvider provider;

    private TMP_Text text;

    void Awake()
    {
        provider.AddListener(x => text.text = ("f(x,y,z) = " + x.ToParsableString()));
    }

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }
}
