using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerOverlay : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons, _Analog;

    [Header("References")]
    [SerializeField] private Image xButton;
    [SerializeField] private Image circleButton;
    [SerializeField] private Image squareButton;
    [SerializeField] private Image triangleButton;

    [SerializeField] private Image l1Button;
    [SerializeField] private Image l2Button;
    [SerializeField] private Image r1Button;
    [SerializeField] private Image r2Button;

    [SerializeField] private Image l3Analog;
    [SerializeField] private Image r3Analog;

    [SerializeField] private Color highlightColor;
    [SerializeField] private Color defaultColor = Color.white;

    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");
        _Buttons = map.FindAction("Buttons");
        _Analog = map.FindAction("Analog");
        _Analog.canceled += ctx =>
        {
            l3Analog.transform.localPosition = new Vector3(0f, 0f, 0f);
            r3Analog.transform.localPosition = new Vector3(0f, 0f, 0f);
        };

        controls.Enable();
    }

    private void Update()
    {
        ResetColors();

        if (!_Buttons.enabled) return;
        CheckButtons();
        CheckAnalog();
    }

    private void CheckAnalog()
    {
        foreach (var stick in _Analog.controls)
        {
            if (stick.IsPressed())
            {
                Vector2 dir = _Analog.ReadValue<Vector2>();
                if (stick.name == "leftStick")
                {
                    l3Analog.transform.localPosition = new Vector3(dir.x * 50f, dir.y * 50f, 0f);
                    Debug.Log($"Left Stick Direction: {dir}");
                }
                else if()
                {
                    r3Analog.transform.localPosition = new Vector3(dir.x * 50f, dir.y * 50f, 0f);
                }
            }

        }
    }

    private void CheckButtons()
    {
        foreach (var control in _Buttons.controls)
        {
            if (control.IsPressed())
            {
                switch (control.name)
                {
                    case "buttonSouth":
                        xButton.color = highlightColor;
                        break;
                    case "buttonEast":
                        circleButton.color = highlightColor;
                        break;
                    case "buttonWest":
                        squareButton.color = highlightColor;
                        break;
                    case "buttonNorth":
                        triangleButton.color = highlightColor;
                        break;
                    case "leftShoulder":
                        l1Button.color = highlightColor;
                        break;
                    case "L2":
                        l2Button.color = highlightColor;
                        break;
                    case "rightShoulder":
                        r1Button.color = highlightColor;
                        break;
                    case "R2":
                        r2Button.color = highlightColor;
                        break;
                }
            }
        }

    }

    private void ResetColors()
    {
        if (xButton) xButton.color = defaultColor;
        if (circleButton) circleButton.color = defaultColor;
        if (squareButton) squareButton.color = defaultColor;
        if (triangleButton) triangleButton.color = defaultColor;

        if (l1Button) l1Button.color = defaultColor;
        if (l2Button) l2Button.color = defaultColor;
        if (r1Button) r1Button.color = defaultColor;
        if (r2Button) r2Button.color = defaultColor;
    }
}
