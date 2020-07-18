using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TMP_Text))]
[ExecuteAlways]
public class SliderValueDisplay : MonoBehaviour
{
    public string prefix;

    public Slider slider;

    private TMPro.TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();

        text.text = prefix + slider.value;
        slider.onValueChanged.AddListener(e => text.text = prefix + slider.value);
    }
}
