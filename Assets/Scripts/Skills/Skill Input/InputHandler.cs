using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public UnityAction<SkillInput> OnSkillInputReceived;

    //Input Action variables
    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons, _Hold, _HoldL3, _HoldR3, _FlickL3, _FlickR3, _RotateR3, _DiagonalFlick, _AnalogButtons, _HoldL1, _HoldR1, _DiagonalHold;

    //Control States
    [SerializeField] private bool holdDisabled = false;
    [SerializeField] private bool waitingForRelease;
    [SerializeField] private bool isHeld;

    //Input Data
    private List<SkillInput> inputs = new();
    private SkillInput currentInput;

    private Dictionary<string, bool> buttonHoldState = new();

    private void Awake()
    {
        SetupControls();
        RegisterControlCallbacks();
        controls.Enable();
    }

    #region Input Setup & Initialization
    private void SetupControls()
    {
        InputActionMap map = controls.FindActionMap("DualShock");

        _Buttons = map.FindAction("Buttons");
        _Hold = map.FindAction("Hold");
        _HoldL3 = map.FindAction("HoldL3Direction");
        _HoldR3 = map.FindAction("HoldR3Direction");
        _HoldL1 = map.FindAction("HoldL1");
        _HoldR1 = map.FindAction("HoldR1");
        _FlickL3 = map.FindAction("FlickL3");
        _FlickR3 = map.FindAction("FlickR3");
        _RotateR3 = map.FindAction("RotateR3");
        _AnalogButtons = map.FindAction("AnalogButtons");
        _DiagonalFlick = map.FindAction("DiagonalFlick");

        _DiagonalHold = map.FindAction("DiagonalHold");
    }

    private void RegisterControlCallbacks()
    {
        // Buttons control
        _Buttons.performed += ctx => { ProcessInput(ctx.control.name, isHeld: false); };

        //// Analog Buttons control
        _AnalogButtons.performed += ctx => { ProcessInput(ctx.control.name, isHeld: false); };

        // Hold control
        _Hold.performed += ctx => { if (!holdDisabled) HandleHoldStart(ctx.control.name); };

        _Hold.canceled += ctx => { HandleHoldEnd(ctx); };

        _HoldL1.performed += ctx => { if (!holdDisabled) HandleHoldStart(ctx.control.name); };

        _HoldL1.canceled += ctx => { HandleHoldEnd(ctx); };

        _HoldR1.performed += ctx => { if (!holdDisabled) HandleHoldStart(ctx.control.name); };

        _HoldR1.canceled += ctx => { HandleHoldEnd(ctx); };

        // Rotation control
        _RotateR3.performed += ctx => { inputs = GetRotatingInput(ctx.ReadValue<Vector2>()); };
        _RotateR3.canceled += ctx => { HandleRotationEnd(ctx); };
        float deadzone = 0.2f;
        float flickThreshold = 0.4f;
        float flickSpeedThreshold = 1f;
        Vector2 previousInput = Vector2.zero;
        Vector2 smoothedInput = Vector2.zero;
        float smoothingFactor = 0.05f;


        bool isFlicking = false;
        bool isInputStarted = false;

        _DiagonalFlick.performed += ctx =>
        {
            if (isInputStarted) return;
            Vector2 input = ctx.ReadValue<Vector2>();


            if (input.magnitude < deadzone)
            {
                input = Vector2.zero;
            }


            float inputChangeMagnitude = Vector2.Distance(input, previousInput);


            if (inputChangeMagnitude > flickSpeedThreshold)
            {
                Debug.Log("Ignoring fast flick");
                return;
            }

            if (!isFlicking)
            {
                if (input.magnitude > flickThreshold)
                {
                    smoothedInput = input;
                    isFlicking = true;

                    if (ctx.control.name == "leftStick")
                        HandleFlickInputDiagonal(smoothedInput, isLeft: true, isHeld: false);
                    else HandleFlickInputDiagonal(smoothedInput, isLeft: false, isHeld: false);



                    isInputStarted = true;
                }
            }
            else
            {
                smoothedInput = Vector2.Lerp(smoothedInput, input, smoothingFactor);
                HandleFlickInputDiagonal(smoothedInput, isLeft: true, isHeld: false);
            }

            previousInput = input;
        };

        _DiagonalFlick.canceled += ctx =>
        {
            if (isFlicking)
            {
                isFlicking = false;
                isInputStarted = false;
                Debug.Log("Flick ended.");
            }

            smoothedInput = Vector2.zero;
        };

        _DiagonalHold.performed += ctx =>
        {
            if (ctx.control.name == "leftStick")
                HandleFlickInputDiagonal(ctx.ReadValue<Vector2>(), isLeft: true, isHeld: true);
            else HandleFlickInputDiagonal(ctx.ReadValue<Vector2>(), isLeft: false, isHeld: true);
        };
    }
    #endregion

    #region Input Processing
    private void HandleHoldStart(string controlName)
    {

        if (!buttonHoldState.ContainsKey(controlName) || !buttonHoldState[controlName])
        {
            ProcessInput(controlName, isHeld: true);

            buttonHoldState[controlName] = true;
        }
    }

    private void HandleHoldEnd(InputAction.CallbackContext ctx)
    {
        string controlName = ctx.control.name;

        if (ctx.ReadValue<float>() == 0)
        {
            EventManager.OnSkillInputReceived?.Invoke(SkillInput.Hold_None);
            if (buttonHoldState.ContainsKey(controlName))
            {
                buttonHoldState[controlName] = false;
            }
        }
    }

    private void HandleRotationEnd(InputAction.CallbackContext ctx)
    {
        SkillInput? input = SkillInputs.GetStickRotationInput(ctx.control.name, inputs);
        if (input.HasValue)
        {
            EventManager.OnSkillInputReceived?.Invoke(input.Value);
        }
        inputs.Clear();
        currentInput = SkillInput.None;
    }

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
        const float threshold = 0.9f;

        SkillInput? newInput = null;

        if (dir.y >= threshold)
            newInput = SkillInput.R3_Up;
        else if (dir.x >= threshold)
            newInput = SkillInput.R3_Right;
        else if (dir.y <= -threshold)
            newInput = SkillInput.R3_Down;
        else if (dir.x <= -threshold)
            newInput = SkillInput.R3_Left;

        if (newInput.HasValue && currentInput != newInput.Value)
        {
            inputs.Add(newInput.Value);
            currentInput = newInput.Value;
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

    private void HandleFlickInputDiagonal(Vector2 stickPosition, bool isLeft, bool isHeld)
    {
        SkillInput? input = isLeft ?
            SkillInputs.GetFlickDiagonalInput(stickPosition, isLeft: true, isHeld: isHeld) :
            SkillInputs.GetFlickDiagonalInput(stickPosition, isLeft: false, isHeld: isHeld);

        if (input.HasValue)
        {
            EventManager.OnSkillInputReceived?.Invoke(input.Value);
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

