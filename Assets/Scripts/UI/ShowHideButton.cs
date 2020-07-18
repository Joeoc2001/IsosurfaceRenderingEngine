using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class ShowHideButton : MonoBehaviour
{
    private Button button;
    private Image image;
    public ShowHideableCanvas canvas;

    public static Color INACTIVE_COLOUR = new Color(0.8f, 0.8f, 0.8f, 0.4f);
    public static Color ACTIVE_COLOUR = Color.white;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(() => Toggle());
    }

    private void Start()
    {
        SetColour();
    }

    private void Toggle()
    {
        canvas.Toggle();
        SetColour();
    }

    private void SetColour()
    {
        image.color = canvas.IsHiding() ? INACTIVE_COLOUR : ACTIVE_COLOUR;
    }
}
