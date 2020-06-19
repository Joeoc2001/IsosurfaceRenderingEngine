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
        text = GetComponent<TMP_Text>();
        provider.AddListener(x => text.text = ("f(x,y,z) = " + x.ToString()));
    }

    private void Start()
    {
    }
}
