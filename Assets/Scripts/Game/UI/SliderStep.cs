using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
[ExecuteAlways]
public class SliderStep : MonoBehaviour
{
    public float step;

    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(e => StepSlider());
    }

    private void StepSlider()
    {
        if (step == 0)
        {
            step = slider.minValue;
        }

        float newVal = (int)((slider.value - slider.minValue) / step) * step + slider.minValue;
        slider.value = newVal;
    }
}
