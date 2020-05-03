using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public static Universe Instance { get; private set; }

    public Player mainPlayer;

    private void Awake()
    {
        Instance = this;
    }
}
