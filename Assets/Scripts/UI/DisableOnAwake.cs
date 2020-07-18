using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
    public Behaviour component;

    void Awake()
    {
        component.enabled = false;
    }
}
