using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
    [SerializeField][Range(0.3f, 1f)] private float holdDetectionTime;

    private List<SkillInput> inputs = new();
    private SkillInput? currentInput;

    //Buttons state
    private Dictionary<string, bool> buttonHoldState = new();
    private Dictionary<string, float> holdTimers = new();

    private bool isHeld = false;
    private bool isRotated = false;

    private float nTime;

    private bool isFlicked = false;

    private bool timeStarrted = false;

    List<SkillInput?> currentInputs = new();


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


    private void RegisterControlCallbacks()
    {
        // Buttons control
        //_Buttons.performed += ctx => { ProcessInput(ctx.control.name, isHeld: false); };

        //// Analog Buttons control
        _AnalogButtons.performed += ctx => { ProcessInput(ctx.control.name, isHeld: false); };

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

        _Analog.performed += ctx =>
        {
            if (isFlicked)
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
                HandleFlickInput(ctx, lastInput);
                Debug.Log($"[FLICKED] Step1: Input detected = {currentInput}");

                if (currentInput != SkillInput.Flick_None && !currentInputs.Contains(currentInput))
                {
                    if (currentInputs.Count < 1)
                    {
                        currentInputs.Add(currentInput);
                    }
                }
            }

            foreach (var input in currentInputs)
            {
                Debug.Log($"→ {input}");
            }

            Debug.Log($"[FINAL] Current Input: {currentInput}");

            isRotated = false;
            isHeld = false;
            isFlicked = false;
            timeStarrted = false;
            nTime = 0;

            if (currentInputs.Count > 0)
            {
                EventManager.OnMultipleInputsSent?.Invoke(currentInputs);
            }

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

            string path = _Analog.activeControl.path;
            bool isRightStick = path.Contains("rightStick");

            inputs = GetRotatingInput(_Analog.ReadValue<Vector2>(), isRightStick);

            if (inputs.Count > 1)
            {
                // is rotated
                isRotated = true;

                isHeld = false;
                isFlicked = false;
            }
            else if (nTime > holdDetectionTime)
            {
                //is held
                isRotated = false;

                isHeld = true;

                isFlicked = false;
            }
            else if (nTime <= holdDetectionTime)
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
        HandleButtonHoldTimers();
    }
    

    private void HandleButtonHoldTimers()
    {
        foreach (var control in _Hold.controls)
        {
            var button = control as ButtonControl;
            if (button == null) continue;

            string name = control.name;

            if (button.isPressed)
            {
                if (!holdTimers.ContainsKey(name))
                {
                    holdTimers[name] = 0f;
                    buttonHoldState[name] = false;
                }

                holdTimers[name] += Time.deltaTime;

                if (!buttonHoldState[name] && holdTimers[name] >= holdDetectionTime)
                {
                    Debug.Log($"[HOLD DETECTED] {name} for {holdTimers[name]:0.00}s");
                    ProcessInput(name, isHeld: true);
                    buttonHoldState[name] = true;
                }
            }
            else
            {
                if (holdTimers.ContainsKey(name))
                {
                    bool wasHeld = buttonHoldState[name];

                    if (!wasHeld)
                    {
                        Debug.Log($"[TAP DETECTED] {name}");
                        ProcessInput(name, isHeld: false);
                    }
                    else
                    {
                        EventManager.OnSkillInputReceived?.Invoke(SkillInput.Hold_None);
                    }

                    holdTimers.Remove(name);
                    buttonHoldState[name] = false;
                }
            }
        }
    }

    #region Input Processing
    private void HandleHoldStart(string controlName)
    {
        Debug.Log($"Hold started: {controlName}");
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

    private void ProcessInput(string buttonName, bool isHeld = false)
    {        
        if (holdDisabled) return;
        SkillInput? input = isHeld ? SkillInputs.GetHoldInput(buttonName) : SkillInputs.GetTabInput(buttonName);
        if (input.HasValue)
        {
            if (input != SkillInput.None && input != SkillInput.Flick_None && input != SkillInput.Hold_L3_None && input != SkillInput.Hold_None &&
                input != SkillInput.Hold_R3_None && input != SkillInput.L2_None && input != SkillInput.R2_None && input != SkillInput.L3_None && input != SkillInput.R3_None)
            {
                EventManager.OnSkillInputReceived?.Invoke(input.Value);
            }
        }
    }

    private void HandleFlickInput(InputAction.CallbackContext ctx, Vector2 input)
    {
        if (ctx.control.name == "leftStick")
            GetFlickInput(input, isLeft: true, isHeld: false);
        else
            GetFlickInput(input, isLeft: false, isHeld: false);
    }

    private void HandleRotationEnd(InputAction.CallbackContext ctx)
    {
        SkillInput? input = SkillInputs.GetStickRotationInput(ctx.control.name, inputs);
        if (input.HasValue)
        {
            currentInput = input;
        }
    }

    private List<SkillInput> GetRotatingInput(Vector2 dir, bool isRightStick)
    {
        const float threshold = 0.9f;
        SkillInput? newInput = null;

        if (dir.y >= threshold)
            newInput = isRightStick ? SkillInput.R3_Up : SkillInput.L3_Up;
        else if (dir.x >= threshold)
            newInput = isRightStick ? SkillInput.R3_Right : SkillInput.L3_Right;
        else if (dir.y <= -threshold)
            newInput = isRightStick ? SkillInput.R3_Down : SkillInput.L3_Down;
        else if (dir.x <= -threshold)
            newInput = isRightStick ? SkillInput.R3_Left : SkillInput.L3_Left;

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

