using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class MainMenuReturnButton : MonoBehaviour
{
    private UnityEngine.UI.Button button;
    public MainMenuControl control;

    private void Awake()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(() => control.Toggle());
    }
}
