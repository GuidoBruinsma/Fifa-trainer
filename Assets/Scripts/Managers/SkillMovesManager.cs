using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillMovesManager : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;
    private InputAction _Buttons;
    private InputAction _L;
    private InputAction _R;


    [Space]
    [SerializeField] private Skill currentSkillMove;

    [SerializeField] private List<Skill> skillMoves;    //Actual skill moves SO info

    [SerializeField] private List<SkillInput> currentSequenceInput;  //The Move Input sequence

    [SerializeField] private List<SkillInput> pressedSequenceInput = new();

    private float elapsedTime;

    private int currentSequenceIndex = 0;

    private void Awake()
    {

        // Points to the same list in the memory. If it changed, will change every list. Otherwise use new(currentSkillMove.inputSequence). It will create a copy

        InputActionMap map = controls.FindActionMap("DualShock");
        _Buttons = map.FindAction("Buttons");
        _L = map.FindAction("L");
        _R = map.FindAction("R");


        _Buttons.performed += ctx =>
        {
            DetectInput(ctx.control.name);
            CheckValidity();
        };

        controls.Enable();
    }

    void DetectInput(string buttonName)
    {
        SkillInput _button;

        if (buttonName == "buttonSouth")
        {
            _button = SkillInput.Button_X;
            pressedSequenceInput.Add(_button);
            Debug.Log("X has been pressed");
        }
        else if (buttonName == "buttonEast")
        {
            _button = SkillInput.Button_Circle;
            pressedSequenceInput.Add(_button);

            Debug.Log("O has been pressed");
        }
        else if (buttonName == "buttonWest")
        {
            _button = SkillInput.Button_Square;
            pressedSequenceInput.Add(_button);

            Debug.Log("Square has been pressed");
        }
        else if (buttonName == "buttonNorth")
        {
            _button = SkillInput.Button_Triangle;
            pressedSequenceInput.Add(_button);

            Debug.Log("Triangle has been pressed");
        }
    }

    void CheckValidity()
    {
        currentSkillMove = skillMoves[currentSequenceIndex];
        currentSequenceInput = currentSkillMove.inputSequence;


        if (pressedSequenceInput.Count > currentSequenceInput.Count)
        {
            pressedSequenceInput.Clear();
            Debug.Log("too many inputs, clearing the pressed buttons");
        }
        for (int i = 0; i < pressedSequenceInput.Count; i++)
        {
            if (pressedSequenceInput[i] == currentSequenceInput[i])
            {
                Debug.Log("all good");
            }
            else { FailedAttempt(); }
        }
        if (pressedSequenceInput.Count == currentSequenceInput.Count)
        {
            if (currentSequenceIndex <= skillMoves.Count)
                currentSequenceIndex++;
            Debug.Log("skill move done");
        }

    }

    private void OnDisable() => controls.Disable();


    private void Start()
    {
    }

    private void Update()
    {
    }

    private void FailedAttempt()
    {
        pressedSequenceInput.Clear();
        Debug.Log("attempt failed");
    }
}
