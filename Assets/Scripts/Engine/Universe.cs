using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public static readonly double DELTA_RATIO = 1;

    public static Universe Instance { get; private set; }

    public double FPSAim;

    public double TimeDeltaAim { get => 1 / FPSAim; }

    public double TimeDeltaAverage = .017;
    public double FPSAverage = 60;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        double ratioScaled = DELTA_RATIO * Time.deltaTime;
        TimeDeltaAverage *= (1 - ratioScaled);
        TimeDeltaAverage += Time.deltaTime * ratioScaled;

        FPSAverage = 1 / TimeDeltaAverage;
    }
}
