using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (SkillsValidator), typeof (InputHandler))]
public class SkillMovesManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillsValidator sequenceValidator;
    [SerializeField] private InputHandler inputHandler;
    [Space]
    [SerializeField] private List<Skill> skillMoves;
    private int currentSequenceIndex = 0;
    [SerializeField] private Skill currentSkill;


    private void Start()
    {
        if (skillMoves.Count <= 0) return;

        sequenceValidator = GetComponent<SkillsValidator>();
        inputHandler = GetComponent<InputHandler>();

        sequenceValidator.OnSequenceSuccess += HandleSequenceSuccess;
        sequenceValidator.OnSequenceFailed += HandleSequenceFail;

        LoadCurrentSkillMove();
    }

    private void LoadCurrentSkillMove()
    {
        currentSkill = skillMoves[currentSequenceIndex];

        sequenceValidator.SetSequenceInput(currentSkill.inputSequence);
        inputHandler.ResetHold();

        sequenceValidator.sq.VisualizeSequence(currentSkill.inputSequence, 0);
    }

    private void SetCurrentSkillMove()
    {
        if (currentSequenceIndex < skillMoves.Count - 1)
        {
            currentSequenceIndex++;
            LoadCurrentSkillMove();
        }
        else
        {
            Debug.Log("All skill moves completed!");
        }
    }

    private void HandleSequenceFail()
    {
        Debug.Log("Skill move failed. Resetting sequence.");
    }

    private void HandleSequenceSuccess()
    {
        //sequenceValidator.pressedSequenceInput.Clear();

        inputHandler.CancelHold();

        if (currentSequenceIndex < skillMoves.Count - 1)
        {
            SetCurrentSkillMove();
        }
        else
        {
            sequenceValidator.ResetSequence();
            Debug.Log("All skill moves completed!");
        }
    }

    private void OnDisable()
    {
        sequenceValidator.OnSequenceSuccess -= HandleSequenceSuccess;
        sequenceValidator.OnSequenceFailed -= HandleSequenceFail;
    }
}
