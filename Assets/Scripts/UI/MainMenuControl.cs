using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    public SmoothMouseLook MouseLookLock;
    public SmoothWASDMove WASDMoveLock;

    public ShowHideableCanvas MainMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        MainMenu.Toggle();

        Cursor.lockState = MainMenu.IsShowing() ? CursorLockMode.None : CursorLockMode.Locked;

        MouseLookLock.enabled = MainMenu.IsHiding();
        WASDMoveLock.enabled = MainMenu.IsHiding();
    }
}
