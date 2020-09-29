using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
    void Awake()
    {
        this.enabled = false;
    }
}
