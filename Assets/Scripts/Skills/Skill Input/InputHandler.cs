using System.Collections;
using System.Collections.Generic;
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
    private InputAction _Special;

    private bool holdDisabled = false;
    private bool waitingForRelease;

    private List<SkillInput> test = new();     //TODO: Change name later

    private const float holdThreshold = 0.5f;  //TODO: Adapt this how fast should detect a hold

    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");

        _Buttons = map.FindAction("Buttons");
        _Hold = map.FindAction("Hold");
        _Sticks = map.FindAction("Sticks");
        _Special = map.FindAction("Special");

        //Buttons control
        _Buttons.performed += ctx => { ProcessInput(ctx.control.name, ctx); };
        _Special.performed += ctx => { ProcessInput(ctx.control.name, ctx); };
        _Special.canceled += ctx =>  { EventManager.OnSkillInputReceived?.Invoke(SkillInput.RS_None); };

        //Hold control
        _Hold.started += ctx => { if (!holdDisabled) ProcessInput(ctx.control.name, ctx); };
        _Hold.canceled += ctx => { ProcessInput(ctx.control.name, ctx); };

        //Stick control
        _Sticks.performed += ctx =>
        {
            if (!ProcessRotationInput(ctx.ReadValue<Vector2>()))
                ProcessStickInput(ctx.ReadValue<Vector2>());
        };
        _Sticks.canceled += ctx => { ProcessStickInput(ctx.ReadValue<Vector2>()); };

        controls.Enable();
    }

    #region Input Process
    void ProcessInput(string buttonName, InputAction.CallbackContext ctx)
    {
        if (holdDisabled) return;

        if (buttonName == "leftShoulder")
        {
            StartCoroutine(CheckShoulderHold(ctx, SkillInput.L1, SkillInput.L1_Hold));
        }
        else if (buttonName == "rightShoulder")
        {
            StartCoroutine(CheckShoulderHold(ctx, SkillInput.R1, SkillInput.R1_Hold));
        }
        else
        {
            SkillInput? input = GetSkillInput(buttonName);
            if (input.HasValue)
            {
                EventManager.OnSkillInputReceived?.Invoke(input.Value);
            }
        }
    }

    void ProcessStickInput(Vector2 stickVector)
    {
        var degrees = Mathf.Atan2(stickVector.y, stickVector.x) * Mathf.Rad2Deg;

        if (degrees < 0)
            degrees += 360;

        SkillInput input = GetSkillStickInput(degrees, stickVector.magnitude);
        EventManager.OnSkillInputReceived?.Invoke(input);
    }

    private bool ProcessRotationInput(Vector2 stickVector)
    {
        float degrees = Mathf.Atan2(stickVector.y, stickVector.x) * Mathf.Rad2Deg;
        if (degrees < 0)
            degrees += 360;

        SkillInput input = GetSkillStickInput(degrees, stickVector.magnitude);

        Skill currentSkill = SkillMovesManager.CurrentSkill;
        if (stickVector.magnitude == 0)
        {
            test.Clear();
            return false;
        }
        if (currentSkill != null &&
            currentSkill.rotationSequence != null &&
            currentSkill.rotationSequence.Length > 0)
        {
            int expectedIndex = test.Count;

            if (expectedIndex < currentSkill.rotationSequence.Length)
            {
                if (test.Count == 0 || test[test.Count - 1] != input)
                {
                    if (input == currentSkill.rotationSequence[expectedIndex])
                    {
                        test.Add(input);
                        Debug.Log("Matched: " + input + " (expected " + currentSkill.rotationSequence[expectedIndex] + ")");
                    }
                    else
                    {
                        Debug.Log("Mismatch: expected " + currentSkill.rotationSequence[expectedIndex] + " but got " + input);
                        test.Clear();
                        EventManager.OnSkillInputReceived?.Invoke(input);
                        return true;
                    }
                }

                if (test.Count == currentSkill.rotationSequence.Length)
                {
                    Debug.Log("Rotation gesture complete");
                    EventManager.OnSkillInputReceived?.Invoke(SkillInput.R3_Rotate);
                    test.Clear();
                    return true;
                }
                return true;
            }
        }
        return false;
    }

    private IEnumerator CheckShoulderHold(InputAction.CallbackContext ctx, SkillInput tabInput, SkillInput holdInput)
    {
        float startTime = Time.time;
        bool holdEventFired = false;

        while (ctx.ReadValue<float>() > 0)
        {
            float holdTime = Time.time - startTime;
            if (!holdEventFired && holdTime >= holdThreshold)
            {
                Debug.Log("Left Shoulder Held for: " + holdTime + " seconds");
                EventManager.OnSkillInputReceived?.Invoke(holdInput);
                holdEventFired = true;
            }
            yield return null;
        }

        if (!holdEventFired)
        {
            float holdTime = Time.time - startTime;
            Debug.Log("Left Shoulder Tapped for: " + holdTime + " seconds");
            EventManager.OnSkillInputReceived?.Invoke(tabInput);
        }
    }
    #endregion

    #region Hold Control
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

    #endregion

    SkillInput GetSkillStickInput(float angle, float magnitude, float tolerance = -1)
    {
        if (magnitude == 0)
            return SkillInput.RS_None;

        if (tolerance <= 0f)
        {
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
        else
        {
            if (angle >= (360f - tolerance) || angle < (0f + tolerance))
                return SkillInput.RS_Right;
            else if (angle >= (45f - tolerance) && angle < (45f + tolerance))
                return SkillInput.RS_UpRight;
            else if (angle >= (90f - tolerance) && angle < (90f + tolerance))
                return SkillInput.RS_Up;
            else if (angle >= (135f - tolerance) && angle < (135f + tolerance))
                return SkillInput.RS_UpLeft;
            else if (angle >= (180f - tolerance) && angle < (180f + tolerance))
                return SkillInput.RS_Left;
            else if (angle >= (225f - tolerance) && angle < (225f + tolerance))
                return SkillInput.RS_DownLeft;
            else if (angle >= (270f - tolerance) && angle < (270f + tolerance))
                return SkillInput.RS_Down;
            else if (angle >= (315f - tolerance) && angle < (315f + tolerance))
                return SkillInput.RS_DownRight;
            else
                return SkillInput.RS_None;
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

            ("leftShoulder") => 0,
            ("rightShoulder") => 0,

            ("x") => SkillInput.Button_X,
            _ => null
        };
    }

    private void OnDisable() => controls.Disable();
}

