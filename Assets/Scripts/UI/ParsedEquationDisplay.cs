using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParsedEquationDisplay : MonoBehaviour
{
    public EquationProvider provider;

    private Text text;

    void Awake()
    {
        provider.AddListener(x => text.text = (" = " + x.ToString()));
    }

    private void Start()
    {
        text = GetComponent<Text>();
    }
}
