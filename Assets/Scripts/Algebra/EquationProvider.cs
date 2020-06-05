using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EquationProvider : MonoBehaviour
{
    protected readonly OnEquationChange onEquationChange = new OnEquationChange();

    public class OnEquationChange : UnityEvent<Equation>
    {
    }

    public abstract Equation GetEquation();

    public void AddListener(UnityAction<Equation> call)
    {
        onEquationChange.AddListener(call);
    }

    public void RemoveListener(UnityAction<Equation> call)
    {
        onEquationChange.RemoveListener(call);
    }
}
