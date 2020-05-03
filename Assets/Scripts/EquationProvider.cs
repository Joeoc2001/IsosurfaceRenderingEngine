using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EquationProvider : MonoBehaviour
{
    private Equation equation = Constant.ZERO;

    private OnEquationChange onEquationChange = new OnEquationChange();

    class OnEquationChange : UnityEvent<Equation>
    {
        public OnEquationChange()
        {
        }
    }

    protected void SetEquation(Equation e)
    {
        equation = e;
        onEquationChange.Invoke(e);
    }

    public Equation GetEquation()
    {
        return equation;
    }

    public void AddListener(UnityAction<Equation> call)
    {
        onEquationChange.AddListener(call);
    }

    public void RemoveListener(UnityAction<Equation> call)
    {
        onEquationChange.RemoveListener(call);
    }
}
