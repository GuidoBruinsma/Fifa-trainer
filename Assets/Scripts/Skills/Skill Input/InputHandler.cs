using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public UnityAction<SkillInput> OnSkillInputReceived;

    //Input Action variables
    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons, _Hold, _AnalogFlick, _AnalogButtons, _HoldL1, _HoldR1, _AnalogHold, _AnalogRotation;

    //Control States
    [SerializeField] private bool holdDisabled = false;
    [SerializeField] private bool waitingForRelease;
    [SerializeField] private bool isHeld;

    //Input Data
    [SerializeField] private List<SkillInput> inputs = new();
    private SkillInput? currentInput;
    [SerializeField] private SkillInput currentRequiredTypeInput;

    private Dictionary<string, bool> buttonHoldState = new();

    //Analog flick properties
    private float deadzone = 0.2f;
    private float flickThreshold = 0.4f;
    private float flickSpeedThreshold = 1f;
    private Vector2 previousInput = Vector2.zero;
    private Vector2 smoothedInput = Vector2.zero;
    private float smoothingFactor = 0.05f;

    private bool isFlicking = false;
    private Vector2 lastValidInput = Vector2.zero;

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

        _HoldL1 = map.FindAction("HoldL1");
        _HoldR1 = map.FindAction("HoldR1");

        _AnalogButtons = map.FindAction("AnalogButtons");
        _AnalogFlick = map.FindAction("AnalogFlick");

        _AnalogHold = map.FindAction("AnalogHold");
        _AnalogRotation = map.FindAction("AnalogRotation");
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

        List<SkillInput?> currentInputs = new();

        _AnalogHold.performed += ctx =>
        {

            if (ctx.control.name == "leftStick")
                HandleFlickInputDiagonal(ctx.ReadValue<Vector2>(), isLeft: true, isHeld: true);
            else HandleFlickInputDiagonal(ctx.ReadValue<Vector2>(), isLeft: false, isHeld: true);
            if (!currentInputs.Contains(currentInput))
            {
                currentInputs.Add(currentInput);
            }

        };

        _AnalogRotation.performed += ctx =>
        {
            inputs = GetRotatingInput(ctx.ReadValue<Vector2>());
        };

        _AnalogRotation.canceled += ctx =>
        {
            HandleRotationEnd(ctx);
            if (!currentInputs.Contains(currentInput))
            {
                currentInputs.Add(currentInput);
            }
        };

        _AnalogFlick.performed += ctx =>
        {
            HandleDiagonalFlickInput(ctx, isStarted: true);
        };

        _AnalogFlick.canceled += ctx =>
        {
            HandleDiagonalFlickInput(ctx, isStarted: false);
            if (!currentInputs.Contains(currentInput))
            {
                currentInputs.Add(currentInput);
            }

            EventManager.OnMultipleInputsSent?.Invoke(currentInputs);
            currentInputs.Clear();
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


    private void HandleDiagonalFlickInput(InputAction.CallbackContext ctx, bool isStarted)
    {

        Vector2 input = ctx.ReadValue<Vector2>();
        if (isStarted)
        {

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

                    lastValidInput = smoothedInput; // Store the flick input for later execution
                }
            }
            else
            {
                smoothedInput = Vector2.Lerp(smoothedInput, input, smoothingFactor);
                lastValidInput = smoothedInput;
            }

            previousInput = input;
        }
        else
        {
            if (isFlicking)
            {
                isFlicking = false;
                Debug.Log("Flick ended. Executing action.");

                if (ctx.control.name == "leftStick")
                    HandleFlickInputDiagonal(lastValidInput, isLeft: true, isHeld: false);
                else
                    HandleFlickInputDiagonal(lastValidInput, isLeft: false, isHeld: false);
            }

            smoothedInput = Vector2.zero;
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
            currentInput = input;
            //EventManager.OnSkillInputReceived?.Invoke(input.Value);
        }
        inputs.Clear();
        // currentInput = SkillInput.None;
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

        if (newInput.HasValue && (inputs.Count == 0 || inputs.Last() != newInput.Value))
        {
            inputs.Add(newInput.Value);
        }

        currentInput = newInput;
        return inputs;
    }

    private void HandleFlickInputDiagonal(Vector2 stickPosition, bool isLeft, bool isHeld)
    {
        SkillInput? input = isLeft ?
            SkillInputs.GetFlickDiagonalInput(stickPosition, isLeft: true, isHeld: isHeld) :
            SkillInputs.GetFlickDiagonalInput(stickPosition, isLeft: false, isHeld: isHeld);

        if (input.HasValue)
        {
            currentInput = input;
            //   EventManager.OnSkillInputReceived?.Invoke(input.Value);
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

    private void OnDisable()
    {
        controls.Disable();
    }

    public bool IsAnyButtonHeld()
    {
        return buttonHoldState.Values.Contains(true);
    }

}

