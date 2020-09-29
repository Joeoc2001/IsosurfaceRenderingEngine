using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    public SmoothMouseLook MouseLookLock;
    public SmoothWASDMove WASDMoveLock;

    public ShowHideableCanvas[] ShowOnOpen;
    public ShowHideableCanvas[] ShowOnClose;

    public bool IsOpen = false;

    void Start()
    {
        Set();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    internal void Toggle()
    {
        IsOpen = !IsOpen;
        Set();
    }

    public void Set()
    {
        Cursor.lockState = IsOpen ? CursorLockMode.None : CursorLockMode.Locked;
        MouseLookLock.enabled = !IsOpen;
        WASDMoveLock.enabled = !IsOpen;

        foreach (var item in ShowOnOpen)
        {
            if (IsOpen)
            {
                item.Show();
            }
            else
            {
                item.Hide();
            }
        }

        foreach (var item in ShowOnClose)
        {
            if (IsOpen)
            {
                item.Hide();
            }
            else
            {
                item.Show();
            }
        }
    }
}
