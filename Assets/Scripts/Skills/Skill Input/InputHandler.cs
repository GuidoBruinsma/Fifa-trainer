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
    private InputAction _Buttons, _Hold, _AnalogButtons,
        _HoldL1, _HoldR1, _Analog;

    //Control States
    private bool holdDisabled = false;
    private bool waitingForRelease;

    //Input Data
    [SerializeField] private List<SkillInput> inputs = new();
    private SkillInput? currentInput;

    private Dictionary<string, bool> buttonHoldState = new();

    private bool isHeld = false;
    private bool isRotated = false;

    private float nTime;

    private bool isFlicked = false;

    private bool timeStarrted = false;

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

        _Analog = map.FindAction("Analog");
    }

    List<SkillInput?> currentInputs = new();

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


        _Analog.started += ctx =>
        {
            timeStarrted = true;
        };

        Vector2 lastInput = new();

        _Analog.performed += ctx => { 
            if(isFlicked)
                lastInput = ctx.ReadValue<Vector2>();
        };

        _Analog.canceled += ctx =>
        {
            if (isRotated)
            {
                HandleRotationEnd(ctx);
                if (!currentInputs.Contains(currentInput))
                {
                    currentInputs.Add(currentInput);
                    Debug.Log($"Added this input {currentInput}");
                }
            }
            else if (isHeld)
            {
                if (ctx.control.name == "leftStick")
                {
                    GetFlickInput(lastInput, isLeft: true, isHeld: true);
                }
                else
                {
                    GetFlickInput(lastInput, isLeft: false, isHeld: true);
                }

                //Debug.Log($"[HOLD] Stick: {ctx.control.name}, Input: {currentInput}");

                if (!currentInputs.Contains(currentInput))
                {
                    currentInputs.Add(currentInput);
                }

            }
            else if (isFlicked)
            {
                Debug.Log("[FLICKED] Step0: Flick logic starting.");

                HandleFlickInput(ctx, lastInput);
                Debug.Log($"[FLICKED] Step1: Input detected = {currentInput}");

                if (currentInput != SkillInput.Flick_None && !currentInputs.Contains(currentInput))
                {
                    Debug.Log("[FLICKED] Step2: Valid flick input found.");

                    if (currentInputs.Count < 1)
                    {
                        Debug.Log("[FLICKED] Step3: Adding first flick input.");
                        currentInputs.Add(currentInput);
                        Debug.Log($"[FLICKED] Step4: Added input = {currentInput}");
                    }
                }
            }

            // Print all collected inputs
            Debug.Log("[ALL INPUTS]");
            foreach (var input in currentInputs)
            {
                Debug.Log($"→ {input}");
            }

            Debug.Log($"[FINAL] Current Input: {currentInput}");

            // Reset flags
            isRotated = false;
            isHeld = false;
            isFlicked = false;
            timeStarrted = false;
            nTime = 0;

            // Send inputs if any
            if (currentInputs.Count > 0)
            {
                Debug.Log($"[SEND] Sending {currentInputs.Count} inputs to EventManager.");
                EventManager.OnMultipleInputsSent?.Invoke(currentInputs);
            }

            // Clear for next input sequence
            currentInputs.Clear();
            inputs.Clear();
        };
    }
    #endregion

    void AnalogInputType()
    {
        //Debug.Log(_Analog.IsPressed());
        if (_Analog.IsPressed())
        {
            if (timeStarrted)
            {
                nTime += Time.deltaTime;
                //Debug.Log(nTime);
            }

            inputs = GetRotatingInput(_Analog.ReadValue<Vector2>());

            if (inputs.Count > 1)
            {
                // is rotated
                isRotated = true;

                isHeld = false;
                isFlicked = false;
            }
            else if (nTime > 0.3f)
            {
                //is held
                isRotated = false;

                isHeld = true;

                isFlicked = false;
            }
            else if (nTime <= 0.3f)
            {
                isRotated = false;

                isHeld = false;

                isFlicked = true;
            }
        }
    }

    private void Update()
    {
        AnalogInputType();
    }

    #region Input Processing
    private void HandleHoldStart(string controlName)
    {

        if (!buttonHoldState.ContainsKey(controlName) || !buttonHoldState[controlName])
        {
            ProcessInput(controlName, isHeld: true);

            buttonHoldState[controlName] = true;
        }
    }


    private void HandleFlickInput(InputAction.CallbackContext ctx, Vector2 input)
    {
        if (ctx.control.name == "leftStick")
            GetFlickInput(input, isLeft: true, isHeld: false);
        else
            GetFlickInput(input, isLeft: false, isHeld: false);
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
        }
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
        Vector2 rawValue = Gamepad.current.rightStick.ReadValue();
        if (rawValue.y >= threshold)
            newInput = SkillInput.R3_Up;
        else if (rawValue.x >= threshold)
            newInput = SkillInput.R3_Right;
        else if (rawValue.y <= -threshold)
            newInput = SkillInput.R3_Down;
        else if (rawValue.x <= -threshold)
            newInput = SkillInput.R3_Left;

        if (newInput.HasValue && (inputs.Count == 0 || inputs.Last() != newInput.Value))
        {
            inputs.Add(newInput.Value);
        }

        currentInput = newInput;
        return inputs;
    }

    private void GetFlickInput(Vector2 stickPosition, bool isLeft, bool isHeld)
    {
        SkillInput? input = isLeft ?
            SkillInputs.GetFlickDiagonalInput(stickPosition, isLeft: true, isHeld: isHeld) :
            SkillInputs.GetFlickDiagonalInput(stickPosition, isLeft: false, isHeld: isHeld);

        if (input.HasValue)
        {
            currentInput = input;
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

