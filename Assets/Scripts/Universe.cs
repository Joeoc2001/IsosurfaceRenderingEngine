using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public static readonly double DELTA_RATIO = 0.9;

    public static Universe Instance { get; private set; }

    public Player mainPlayer;

    public double FPSAim;

    public double TimeDeltaAim { get => 1 / FPSAim; }

    public double TimeDeltaAverage = .1;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        TimeDeltaAverage *= DELTA_RATIO;
        TimeDeltaAverage += Time.deltaTime * (1 - DELTA_RATIO);
    }
}
