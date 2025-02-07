using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public UnityAction<SkillInput> OnSkillInputReceived;

    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons;
    private InputAction _L;
    private InputAction _R;
    private InputAction _Hold;

    private bool holdDisabled = false;
    private void Awake()
    {
        InputActionMap map = controls.FindActionMap("DualShock");
        _Buttons = map.FindAction("Buttons");
        _L = map.FindAction("L");
        _R = map.FindAction("R");
        _Hold = map.FindAction("Hold");

        _Buttons.performed += ctx => { ProcessInput(ctx.control.name); };

        _Hold.started += ctx => {if(!holdDisabled) ProcessInput(ctx.control.name); };
        _Hold.canceled += ctx => { ProcessInput(ctx.control.name); };

        controls.Enable();
    }

    void ProcessInput(string buttonName)
    {
        if(holdDisabled) return;

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

    public void ResetHold()
    {
        holdDisabled = false;
        _Hold.Enable();
    }

    SkillInput? GetSkillInput(string buttonName)
    {
        return (buttonName) switch
        {
            ("buttonSouth") => SkillInput.Button_X,
            ("buttonEast") => SkillInput.Button_Circle,
            ("buttonWest") => SkillInput.Button_Square,
            ("buttonNorth") => SkillInput.Button_Triangle,
            ("space") => SkillInput.L1_Hold,
            ("x") => SkillInput.Button_X,
            _ => null
        };
    }
    private void OnDisable() => controls.Disable();
}
