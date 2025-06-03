using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Displays a visual overlay for DualShock controller input.
/// Highlights UI elements when buttons, triggers, or sticks are pressed or moved.
/// </summary>
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

    /// <summary>
    /// Finds and enables the controller input action map on Awake.
    /// </summary>
    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");
        _Buttons = map.FindAction("Buttons");
        controls.Enable();
    }

    /// <summary>
    /// Resets button colors and checks for active inputs each frame.
    /// </summary>
    private void Update()
    {
        //ResetColors();

        //CheckAnalog();
        //CheckTriggers();
        //CheckButtons();
        //StickButtons();
    }

    /// <summary>
    /// Checks analog stick movement and updates L3 and R3 position and color.
    /// </summary>
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



    /// <summary>
    /// Checks trigger input (L2, R2) and updates color based on pressure.
    /// </summary>
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

    /// <summary>
    /// Highlights button images based on which buttons are currently pressed.
    /// </summary>
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

    /// <summary>
    /// Highlights the stick button (L3/R3) if pressed, and resets when released.
    /// </summary>
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
            else if (!gamepad.rightStickButton.IsPressed() && rightDir.magnitude < 0.01f) r3Analog.color = defaultColor;

            if (gamepad.leftStickButton.IsPressed())
            {
                l3Analog.color = highlightColor;
            }
            else if (!gamepad.leftStickButton.IsPressed() && leftDir.magnitude < 0.01f) l3Analog.color = defaultColor;

        }
    }

    /// <summary>
    /// Resets all button and trigger images to their default color.
    /// </summary>
    public void ResetColors()
    {
        if (xButton) xButton.color = defaultColor;
        if (circleButton) circleButton.color = defaultColor;
        if (squareButton) squareButton.color = defaultColor;
        if (triangleButton) triangleButton.color = defaultColor;

        if (l1Button) l1Button.color = defaultColor;
        if (l2Trigger) l2Trigger.color = defaultColor;
        if (r1Button) r1Button.color = defaultColor;
        if (r2Trigger) r2Trigger.color = defaultColor;

        //Delete after
        r3Analog.color = defaultColor;
        l3Analog.color = defaultColor;
        l3Analog.transform.localPosition = Vector3.zero;
        r3Analog.transform.localPosition = Vector3.zero;
    }

    //Delete later
    #region Test  
    public void SimulateButton(SkillInput input)
    {
        string name = input.ToString();

        if (name.Contains("None")) return;

        bool isHold = name.Contains("_Hold") || name.StartsWith("Hold_");
        string cleanName = name.Replace("_Hold", "").Replace("Hold_", "");

        // Stick rotations: e.g., R3_LeftToUp → highlight R3 and show direction
        if (cleanName.Contains("To"))
        {
            HighlightStickRotation(cleanName);
            return;
        }

        Color colorToUse = isHold ? Color.red : highlightColor;

        if (cleanName.StartsWith("L2")) l2Trigger.color = colorToUse;
        else if (cleanName.StartsWith("R2")) r2Trigger.color = colorToUse;
        else if (cleanName.StartsWith("L1")) l1Button.color = colorToUse;
        else if (cleanName.StartsWith("R1")) r1Button.color = colorToUse;
        else if (cleanName.StartsWith("Button_X")) xButton.color = colorToUse;
        else if (cleanName.StartsWith("Button_Circle")) circleButton.color = colorToUse;
        else if (cleanName.StartsWith("Button_Square")) squareButton.color = colorToUse;
        else if (cleanName.StartsWith("Button_Triangle")) triangleButton.color = colorToUse;
        else if (cleanName.StartsWith("L3")) l3Analog.color = colorToUse;
        else if (cleanName.StartsWith("R3")) r3Analog.color = colorToUse;
    }
    private void HighlightStickRotation(string name)
    {
        if (name.StartsWith("L3"))
            StartCoroutine(AnimateStickDirection(l3Analog, name.Replace("L3_", "")));
        else if (name.StartsWith("R3"))
            StartCoroutine(AnimateStickDirection(r3Analog, name.Replace("R3_", "")));
    }

    private IEnumerator AnimateStickDirection(Image stick, string direction)
    {
        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;

        float radius = 50f;
        float duration = 0.3f;
        float elapsed = 0f;

        Color originalColor = stick.color;
        stick.color = highlightColor;

        // Simple directions
        switch (direction)
        {
            case "Left":
                start = Vector2.zero;
                end = Vector2.left;
                break;
            case "Right":
                start = Vector2.zero;
                end = Vector2.right;
                break;
            case "Up":
                start = Vector2.zero;
                end = Vector2.up;
                break;
            case "Down":
                start = Vector2.zero;
                end = Vector2.down;
                break;

            // Two-part rotations
            case "LeftToDown":
                start = Vector2.left;
                end = Vector2.down;
                break;
            case "DownToRight":
                start = Vector2.down;
                end = Vector2.right;
                break;
            case "RightToUp":
                start = Vector2.right;
                end = Vector2.up;
                break;
            case "UpToLeft":
                start = Vector2.up;
                end = Vector2.left;
                break;

            // Three-part arcs
            case "LeftToDownToLeft":
                yield return AnimateArc(stick, Vector2.left, Vector2.down, Vector2.left, radius, duration * 1.5f);
                goto Reset;
            case "LeftToUpToLeft":
                yield return AnimateArc(stick, Vector2.left, Vector2.up, Vector2.left, radius, duration * 1.5f);
                goto Reset;
            case "DownToRightToDown":
                yield return AnimateArc(stick, Vector2.down, Vector2.right, Vector2.down, radius, duration * 1.5f);
                goto Reset;
            case "RightToDownToRight":
                yield return AnimateArc(stick, Vector2.right, Vector2.down, Vector2.right, radius, duration * 1.5f);
                goto Reset;
            case "HalfCircleBack":
                yield return AnimateArc(stick, Vector2.right, Vector2.down, Vector2.left, radius, duration * 1.5f);
                goto Reset;
            default:
                Debug.LogWarning("Unknown stick direction: " + direction);
                yield break;
        }

        // Animate simple directions linearly
        Vector2 from = start * radius;
        Vector2 to = end * radius;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            stick.transform.localPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

    Reset:
        stick.transform.localPosition = Vector3.zero;
        stick.color = originalColor;
    }

    private IEnumerator AnimateArc(Image stick, Vector2 p0, Vector2 p1, Vector2 p2, float radius, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector2 point = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
            stick.transform.localPosition = point * radius;
            yield return null;
        }
    }
    
    public void SetStick(SkillInput input)
    {
        var up = new Vector2(0, 1);
        var down = new Vector2(0, -1);
        var left = new Vector2(-1, 0);
        var right = new Vector2(1, 0);

        if (input == SkillInput.L3_Up)
        {
            l3Analog.transform.localPosition = new Vector3(up.x * 50f, up.y * 50f, 0f);
            l3Analog.color = Color.Lerp(defaultColor, highlightColor, up.magnitude);
        }
        if (input == SkillInput.L3_Down)
        {
            l3Analog.transform.localPosition = new Vector3(down.x * 50f, down.y * 50f, 0f);
            l3Analog.color = Color.Lerp(defaultColor, highlightColor, down.magnitude);
        }
        if (input == SkillInput.L3_Left)
        {
            l3Analog.transform.localPosition = new Vector3(left.x * 50f, left.y * 50f, 0f);
            l3Analog.color = Color.Lerp(defaultColor, highlightColor, left.magnitude);
        }
        if (input == SkillInput.L3_Right)
        {
            l3Analog.transform.localPosition = new Vector3(right.x * 50f, right.y * 50f, 0f);
            l3Analog.color = Color.Lerp(defaultColor, highlightColor, right.magnitude);
        }





        if (input == SkillInput.R3_Up)
        {
            Debug.Log($"move input {input}");
            r3Analog.transform.localPosition = new Vector3(up.x * 50f, up.y * 50f, 0f);
            r3Analog.color = Color.Lerp(defaultColor, highlightColor, up.magnitude);
        }
        if (input == SkillInput.R3_Down)
        {
            Debug.Log($"move input {input}");

            r3Analog.transform.localPosition = new Vector3(down.x * 50f, down.y * 50f, 0f);
            r3Analog.color = Color.Lerp(defaultColor, highlightColor, down.magnitude);
        }
        if (input == SkillInput.R3_Left)
        {
            Debug.Log($"move input {input}");

            r3Analog.transform.localPosition = new Vector3(left.x * 50f, left.y * 50f, 0f);
            r3Analog.color = Color.Lerp(defaultColor, highlightColor, left.magnitude);
        }
        if (input == SkillInput.R3_Right)
        {
            Debug.Log($"move input {input}");

            r3Analog.transform.localPosition = new Vector3(right.x * 50f, right.y * 50f, 0f);
            r3Analog.color = Color.Lerp(defaultColor, highlightColor, right.magnitude);
        }
    }

    #endregion
}
