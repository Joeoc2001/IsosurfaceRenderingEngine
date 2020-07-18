using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class ShowHideableCanvas : MonoBehaviour
{
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public void Show()
    {
        if (IsShowing())
        {
            return;
        }
        canvas.enabled = true;
    }

    public void Hide()
    {
        if (IsHiding())
        {
            return;
        }
        canvas.enabled = false;
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
        return canvas.enabled;
    }

    public bool IsHiding()
    {
        return !IsShowing();
    }
}
