using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : EquationProvider
{

    // Start is called before the first frame update
    void Start()
    {
        SetEquation(Equation.Pow(Variable.Z, 1 / 2) + Variable.Y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
