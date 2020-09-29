using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ShowHideableCanvas : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void Show()
    {
        if (IsShowing())
        {
            return;
        }
        _canvas.enabled = true;
    }

    public void Hide()
    {
        if (IsHiding())
        {
            return;
        }
        _canvas.enabled = false;
    }

    public void Toggle()
    {
        if (IsShowing())
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public bool IsShowing()
    {
        return _canvas.enabled;
    }

    public bool IsHiding()
    {
        return !IsShowing();
    }
}
