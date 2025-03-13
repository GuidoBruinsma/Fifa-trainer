using System.Collections.Generic;
using UnityEngine;

public class SkillsValidator : MonoBehaviour
{
    public SequenceVisualizer sq;

    [SerializeField] private List<SkillInput> currentSequenceInput;  //The Move Input sequence
    [SerializeField] private List<SkillInputHolder> currentSequenceInputHolder;  //The Move Input sequence

    [SerializeField] private List<SkillInput> pressedSequenceInput = new();

    private Skill currentSkill;
    private float timeLeftToPress;

    private float currentTime;
    [SerializeField] private int totalAttempts;

    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(AddInput);
        currentSequenceInputHolder = new(currentSkill.inputSequence);
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);
    }

    private void OnDisable() => EventManager.OnSkillInputReceived.RemoveListener(AddInput);

    private void FixedUpdate()
    {
        if (currentSkill == null) return;

        //timeLeftToPress -= Time.fixedDeltaTime;
        UI_Manager.Instance?.SetTimerText(timeLeftToPress);

        if (timeLeftToPress <= 0)
        {
            ResetSequence();
            timeLeftToPress = 0;
            EventManager.OnWholeSessionFailed?.Invoke();
        }
    }

    public void AddInput(SkillInput input)
    {
        if (pressedSequenceInput.Count == 0) currentTime = Time.time;

        if (input == SkillInput.Flick_None)
            return;


        Debug.Log(input);

        pressedSequenceInput.Add(input);
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);

        if (!CheckValidity())
        {
            SequenceFailed();
        }
        else if (currentSequenceInput.Count == pressedSequenceInput.Count)
        {
            EventManager.OnSequenceSuccess?.Invoke();

            currentSequenceInputHolder = new(currentSkill.inputSequence);
            sq.VisualizeSequence(currentSkill.inputSequence, 0);

            float elapsedTime = (Time.time - currentTime);
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);

            ResetSequence();

            GlobalDataManager.SetNewData(currentSkill.moveName, 0.8f);
        }
    }

    private bool CheckValidity()
    {
        int pressedInputIndex = pressedSequenceInput.Count - 1;

        SkillInput pressedInput = pressedSequenceInput[pressedInputIndex];

        foreach (var inputHolder in currentSequenceInputHolder)
        {
            var inputList = inputHolder.input;

            if (inputList[pressedInputIndex] == SkillInput.L3_Any && SkillInputs.IsL3Input(pressedInput))
            {
                return true;
            }
            else if (inputList[pressedInputIndex] == SkillInput.R3_Any && SkillInputs.IsR3Input(pressedInput))
            {
                return true;
            }
            else if (pressedInput == inputList[pressedInputIndex])
            {
                return true;
            }
        }

        return false;
    }

  
    private void SequenceFailed()
    {
        totalAttempts++;
        ResetSequence();
        EventManager.OnSequenceFailed?.Invoke();
        sq.VisualizeSequence(currentSkill.inputSequence, 0);
    }

    public void SetSequenceInput(List<SkillInput> currentSequenceInput, Skill skill)
    {
        this.currentSequenceInput = new(currentSequenceInput);
        currentSkill = skill;
        timeLeftToPress = currentSkill.maxTimeBetweenInput;
    }

    public void ResetSequence() => pressedSequenceInput.Clear();
}
