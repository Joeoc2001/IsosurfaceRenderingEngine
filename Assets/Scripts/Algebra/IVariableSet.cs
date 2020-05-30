using System;
using UnityEngine;

public interface IVariableSet
{
    float this[Variable v]
    {
        get;
    }
}
