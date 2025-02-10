using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public UnityAction<SkillInput> OnSkillInputReceived;

    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons;
    private InputAction _Hold;
    private InputAction _Sticks;

    private bool holdDisabled = false;
    private bool waitingForRelease;

    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");

        _Buttons = map.FindAction("Buttons");
        _Hold = map.FindAction("Hold");
        _Sticks = map.FindAction("Sticks");

        _Buttons.performed += ctx => { ProcessInput(ctx.control.name); };

        _Hold.started += ctx => { if (!holdDisabled) ProcessInput(ctx.control.name); };
        _Hold.canceled += ctx => { ProcessInput(ctx.control.name); };

        _Sticks.performed += ctx => { ProcessStickInput(ctx.ReadValue<Vector2>()); };
        _Sticks.canceled += ctx => { ProcessStickInput(ctx.ReadValue<Vector2>()); };

        controls.Enable();
    }

    void ProcessStickInput(Vector2 stickVector)
    {
        var degrees = Mathf.Atan2(stickVector.y, stickVector.x) * Mathf.Rad2Deg;

        if (degrees < 0)
            degrees += 360;

        SkillInput input = GetSkillStickInput(degrees, stickVector.magnitude);
        Debug.Log(input);
        EventManager.OnSkillInputReceived?.Invoke(input);
    }

    SkillInput GetSkillStickInput(float angle, float magnitude)
    {
        if (magnitude == 0)
            return SkillInput.RS_None;

        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        if (snappedAngle >= 360f)
            snappedAngle = 0f;

        if (snappedAngle == 0f)
            return SkillInput.RS_Right;
        else if (snappedAngle == 45f)
            return SkillInput.RS_UpRight;
        else if (snappedAngle == 90f)
            return SkillInput.RS_Up;
        else if (snappedAngle == 135f)
            return SkillInput.RS_UpLeft;
        else if (snappedAngle == 180f)
            return SkillInput.RS_Left;
        else if (snappedAngle == 225f)
            return SkillInput.RS_DownLeft;
        else if (snappedAngle == 270f)
            return SkillInput.RS_Down;
        else
            return SkillInput.RS_DownRight;
    }

    void ProcessInput(string buttonName)
    {
        if (holdDisabled) return;

        SkillInput? input = GetSkillInput(buttonName);
        if (input.HasValue)
        {
            EventManager.OnSkillInputReceived?.Invoke(input.Value);
        }
    }

    public void CancelHold()
    {
        holdDisabled = true;
        _Hold.Disable();
    }

    public void CancelHoldAndWaitForRelease()
    {
        holdDisabled = true;
        waitingForRelease = true;
        _Hold.Disable();

        OnSkillInputReceived?.Invoke(SkillInput.RS_None);

        StartCoroutine(WaitForHoldRelease());
    }

    private IEnumerator WaitForHoldRelease()
    {
        while (_Hold.ReadValue<float>() > 0.1f)
        {
            yield return null;
        }
        holdDisabled = false;
        waitingForRelease = false;
        _Hold.Enable();

    }

    public void ResetHold()
    {
        holdDisabled = false;
        if (!waitingForRelease)
        {
            _Hold.Enable();
        }
    }

    SkillInput? GetSkillInput(string buttonName)
    {
        return (buttonName) switch
        {
            ("buttonSouth") => SkillInput.Button_X,
            ("buttonEast") => SkillInput.Button_Circle,
            ("buttonWest") => SkillInput.Button_Square,
            ("buttonNorth") => SkillInput.Button_Triangle,
            ("leftTrigger") => SkillInput.L2_Hold,
            ("rightTrigger") => SkillInput.R2_Hold,

            ("x") => SkillInput.Button_X,
            _ => null
        };
    }

    private void OnDisable() => controls.Disable();
}

