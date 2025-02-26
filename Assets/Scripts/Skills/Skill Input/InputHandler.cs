using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputHandler : MonoBehaviour
{
    public UnityAction<SkillInput> OnSkillInputReceived;
    public List<SkillInput> inputs = new();
    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons;
    private InputAction _Hold;

    private InputAction _HoldL3;
    private InputAction _HoldR3;

    private InputAction _FlickL3;
    private InputAction _FlickR3;

    private InputAction _RotateR3;

    private bool holdDisabled = false;
    private bool waitingForRelease;

    private List<SkillInput> test = new();     //TODO: Change name later

    private const float holdThreshold = 0.5f;  //TODO: Adapt this how fast should detect a hold

    private bool isHeld;

    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");
        //InputActionMap map = controls.FindActionMap("Nintendo");

        _Buttons = map.FindAction("Buttons");
        _Hold = map.FindAction("Hold");

        _HoldL3 = map.FindAction("HoldL3Direction");
        _HoldR3 = map.FindAction("HoldR3Direction");

        _FlickL3 = map.FindAction("FlickL3");
        _FlickR3 = map.FindAction("FlickR3");

        _RotateR3 = map.FindAction("RotateR3");

        //Buttons control
        _Buttons.performed += ctx => { if (!isHeld) ProcessInput(ctx.control.name, isHeld: false); };

        //Hold control
        _Hold.performed += ctx =>
        {
            if (!holdDisabled)
            {
                if (!isHeld)
                {
                    isHeld = true;
                    ProcessInput(ctx.control.name, isHeld: true);
                }
            }
        };
        _Hold.canceled += ctx =>
        {
            if (ctx.ReadValue<float>() == 0)
            {
                EventManager.OnSkillInputReceived?.Invoke(SkillInput.L2_None);
            }
            isHeld = false;
        };

        //TODO URGENT: Add a new element only if the current element is != the the last added element
        _RotateR3.performed += ctx =>
        {
            inputs = GetRotatingInput(ctx.ReadValue<Vector2>());
        };
        _RotateR3.canceled += ctx =>
        {
            SkillInput? input = SkillInputs.GetStickRotationInput(ctx.control.name, inputs, ctx);
            Debug.Log(input);
        };


        //TODO: Send the input to the validator to check if the move is valid
        _HoldL3.performed += ctx => { SkillInput? input = SkillInputs.GetStickInput(ctx.control.name, isLeft: true, isHeld: true); };       // Finished
        _HoldR3.performed += ctx => { SkillInput? input = SkillInputs.GetStickInput(ctx.control.name, isLeft: false, isHeld: true); };      // Finished

        _FlickL3.performed += ctx => { SkillInput? input = SkillInputs.GetStickInput(ctx.control.name, isLeft: true, isHeld: false); };     // Finished
        _FlickR3.performed += ctx => { SkillInput? input = SkillInputs.GetStickInput(ctx.control.name, isLeft: false, isHeld: false); };    // Finished

        controls.Enable();
    }

    #region Input Process
    private void ProcessInput(string buttonName, bool isHeld = false)
    {
        if (holdDisabled) return;

        SkillInput? input = isHeld ? SkillInputs.GetHoldInput(buttonName) : SkillInputs.GetTabInput(buttonName);
        if (input.HasValue)
        {
            EventManager.OnSkillInputReceived?.Invoke(input.Value);
        }
    }

    private List<SkillInput> GetRotatingInput(Vector2 dir)
    {

        if (dir.y >= 0.9f)
        {
            inputs.Add(SkillInput.R3_Up);
        }
        else if (dir.x >= 0.9f)
        {
            inputs.Add(SkillInput.R3_Right);
        }
        else if (dir.y <= -0.9f)
        {
            inputs.Add(SkillInput.R3_Down);
        }
        else if (dir.x <= -0.9f)
        {
            inputs.Add(SkillInput.R3_Left);
        }

        return inputs;
    }

    private void ProcessStickInput(string buttonName, bool isLeft, bool isHeld)
    {
        SkillInput? input;
        if (!isHeld)
        {
            input = isLeft ?
                SkillInputs.GetStickInput(buttonName, isLeft: true, isHeld: false) : SkillInputs.GetStickInput(buttonName, isLeft: false, isHeld: false);
        }
        else
        {
            input = isLeft ?
                SkillInputs.GetStickInput(buttonName, isLeft: true, isHeld: true) : SkillInputs.GetStickInput(buttonName, isLeft: false, isHeld: true);
        }
        if (input.HasValue)
        {
            EventManager.OnSkillInputReceived?.Invoke(input.Value);
        }
    }

    //private void ProcessStickInput(Vector2 stickVector, bool isLeftTrigger)
    //{
    //    var degrees = Mathf.Atan2(stickVector.y, stickVector.x) * Mathf.Rad2Deg;

    //    if (degrees < 0)
    //        degrees += 360;

    //    SkillInput? input = isLeftTrigger ?
    //        SkillInputs.GetSkillStickInput(degrees, stickVector.magnitude, true) : SkillInputs.GetSkillStickInput(degrees, stickVector.magnitude, false);



    //    //FIX: if a flick move is needed this is called and the same problem, analog sends every movement!
    //    EventManager.OnSkillInputReceived?.Invoke(input.Value);
    //}

    //private bool ProcessRotationInput(Vector2 stickVector, bool isLeftTrigger)
    //{
    //    float degrees = Mathf.Atan2(stickVector.y, stickVector.x) * Mathf.Rad2Deg;
    //    if (degrees < 0)
    //        degrees += 360;

    //    SkillInput? input = isLeftTrigger ? SkillInputs.GetSkillStickInput(degrees, stickVector.magnitude, true) : SkillInputs.GetSkillStickInput(degrees, stickVector.magnitude, false);

    //    Skill currentSkill = SkillMovesManager.CurrentSkill;
    //    if (stickVector.magnitude == 0)
    //    {
    //        test.Clear();
    //        return false;
    //    }
    //    if (currentSkill != null &&
    //        currentSkill.rotationSequence != null &&
    //        currentSkill.rotationSequence.Length > 0)
    //    {
    //        int expectedIndex = test.Count;

    //        if (expectedIndex < currentSkill.rotationSequence.Length)
    //        {
    //            if (test.Count == 0 || test[test.Count - 1] != input)
    //            {
    //                if (input == currentSkill.rotationSequence[expectedIndex])
    //                {
    //                    test.Add(input.Value);
    //                }
    //                else
    //                {
    //                    test.Clear();
    //                    //EventManager.OnSkillInputReceived?.Invoke(input);
    //                    return true;
    //                }
    //            }

    //            if (test.Count == currentSkill.rotationSequence.Length)
    //            {
    //                EventManager.OnSkillInputReceived?.Invoke(SkillInput.L3_Rotate);
    //                test.Clear();
    //                return true;
    //            }
    //            return true;
    //        }
    //    }
    //    return false;
    //}

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

        OnSkillInputReceived?.Invoke(SkillInput.R3_None);

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

    private void OnDisable() => controls.Disable();
}

