using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class MainMenuReturnButton : MonoBehaviour
{
    private UnityEngine.UI.Button _button;
    public MainMenuControl control;

    private void Awake()
    {
        _button = GetComponent<UnityEngine.UI.Button>();
        _button.onClick.AddListener(() => control.Toggle());
    }
}
