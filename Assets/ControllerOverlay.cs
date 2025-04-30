using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerOverlay : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons;

    [Header("References")]
    [SerializeField] private Image xButton;
    [SerializeField] private Image circleButton;
    [SerializeField] private Image squareButton;
    [SerializeField] private Image triangleButton;

    [SerializeField] private Image l1Button;
    [SerializeField] private Image l2Trigger;
    [SerializeField] private Image r1Button;
    [SerializeField] private Image r2Trigger;

    [SerializeField] private Image l3Analog;
    [SerializeField] private Image r3Analog;

    [SerializeField] private Color highlightColor;
    [SerializeField] private Color defaultColor = Color.white;

    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");
        _Buttons = map.FindAction("Buttons");
        controls.Enable();
    }

    private void Update()
    {
        ResetColors();

        CheckAnalog();
        CheckTriggers();
        CheckButtons(); 
        StickButtons();
    }

    private void CheckAnalog()
    {
        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            Vector2 leftDir = gamepad.leftStick.ReadValue();
            Vector2 rightDir = gamepad.rightStick.ReadValue();

            if (leftDir.magnitude > 0.01f)
            {
                l3Analog.transform.localPosition = new Vector3(leftDir.x * 50f, leftDir.y * 50f, 0f);
                l3Analog.color = Color.Lerp(defaultColor, highlightColor, leftDir.magnitude);
            }
            else l3Analog.transform.localPosition = Vector3.zero;

            if (rightDir.magnitude > 0.01f)
            {
                r3Analog.transform.localPosition = new Vector3(rightDir.x * 50f, rightDir.y * 50f, 0f);
                r3Analog.color = Color.Lerp(defaultColor, highlightColor, rightDir.magnitude);
            }
            else r3Analog.transform.localPosition = Vector3.zero;

        }
    }

    private void CheckTriggers()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            float l2 = gamepad.leftTrigger.ReadValue();
            float r2 = gamepad.rightTrigger.ReadValue();

            if (l2 > 0.01f)
            {
                l2Trigger.color = Color.Lerp(defaultColor, highlightColor, l2);
            }
            if (r2 > 0.01f)
            {
                r2Trigger.color = Color.Lerp(defaultColor, highlightColor, r2);
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
                    case "rightShoulder":
                        r1Button.color = highlightColor;
                        break;
                }
            }
        }
    }

    private void StickButtons()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 leftDir = gamepad.leftStick.ReadValue();
            Vector2 rightDir = gamepad.rightStick.ReadValue();

            if (gamepad.rightStickButton.IsPressed())
            {
                r3Analog.color = highlightColor;
            }
            else if(!gamepad.rightStickButton.IsPressed() && rightDir.magnitude  < 0.01f) r3Analog.color = defaultColor;

            if (gamepad.leftStickButton.IsPressed())
            {
                l3Analog.color = highlightColor;
            }
            else if (!gamepad.leftStickButton.IsPressed() && leftDir.magnitude < 0.01f) l3Analog.color = defaultColor;

        }
    }

    private void ResetColors()
    {
        if (xButton) xButton.color = defaultColor;
        if (circleButton) circleButton.color = defaultColor;
        if (squareButton) squareButton.color = defaultColor;
        if (triangleButton) triangleButton.color = defaultColor;

        if (l1Button) l1Button.color = defaultColor;
        if (l2Trigger) l2Trigger.color = defaultColor;
        if (r1Button) r1Button.color = defaultColor;
        if (r2Trigger) r2Trigger.color = defaultColor;
    }
}
