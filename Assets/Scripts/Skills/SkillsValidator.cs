using System.Collections.Generic;
using UnityEngine;

public class SkillsValidator : MonoBehaviour
{
    public SequenceVisualizer sq;

    [SerializeField] private List<SkillInput> currentSequenceInput;  //The Move Input sequence
    [SerializeField] private List<SkillInputHolder> currentSequenceInputHolder;  //The Move Input sequence

    [SerializeField] private List<SkillInput?> pressedSequenceInput = new();

    private Skill currentSkill;
    private float timeLeftToPress;

    private float currentTime;
    [SerializeField] private int totalAttempts;
    private SkillInput currentWantedSkill;
    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(AddInput);
        EventManager.OnMultipleInputsSent.AddListener(AddInput);
        currentSequenceInputHolder = new(currentSkill.inputSequence);
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);
    }

    private void OnDisable()
    {
        EventManager.OnSkillInputReceived.RemoveListener(AddInput);
        EventManager.OnMultipleInputsSent.RemoveListener(AddInput);

    }

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
            ResetSequence();
            EventManager.OnSequenceSuccess?.Invoke();

            currentSequenceInputHolder = new(currentSkill.inputSequence);
            sq.VisualizeSequence(currentSkill.inputSequence, 0);

            float elapsedTime = (Time.time - currentTime);
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);


            GlobalDataManager.SetNewData(currentSkill.moveName, 0.8f);
        }
    }
    /// <summary>
    /// FIX: After a few skill moves, it starts sending empty input list. Check isFlicking in input handler. Maybe that could be the problem
    /// 
    /// </summary>
    /// <param name="input"></param>
    public void AddInput(List<SkillInput?> input)
    {
        if (pressedSequenceInput.Count == 0) currentTime = Time.time;

        if (input.Contains(SkillInput.Flick_None))
            return;

        Debug.Log($"Received Inputs: {string.Join(", ", input)}");

        pressedSequenceInput.Add(input[0]); // Store at least one input for sequence tracking
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);

        if (!CheckValidity(input)) // Check batch validity
        {
            SequenceFailed();
        }
        else if (currentSequenceInput.Count == pressedSequenceInput.Count)
        {
            ResetSequence();
            EventManager.OnSequenceSuccess?.Invoke();

            currentSequenceInputHolder = new(currentSkill.inputSequence);
            sq.VisualizeSequence(currentSkill.inputSequence, 0);

            float elapsedTime = (Time.time - currentTime);
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);

            GlobalDataManager.SetNewData(currentSkill.moveName, 0.8f);
        }
    }

    private bool CheckValidity()
    {
        int pressedInputIndex = pressedSequenceInput.Count - 1;

        SkillInput? pressedInput = pressedSequenceInput[pressedInputIndex];

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

    private bool CheckValidity(List<SkillInput?> receivedInputs)
    {
        int pressedInputIndex = pressedSequenceInput.Count - 1;
        if (pressedInputIndex < 0 || pressedInputIndex >= currentSequenceInputHolder[0].input.Count)
            return false;

        foreach (var inputHolder in currentSequenceInputHolder)
        {
            var expectedInput = inputHolder.input[pressedInputIndex];

            foreach (var receivedInput in receivedInputs)
            {
                if (receivedInput == null) continue;

                if (expectedInput == SkillInput.L3_Any && SkillInputs.IsL3Input(receivedInput.Value))
                    return true;

                if (expectedInput == SkillInput.R3_Any && SkillInputs.IsR3Input(receivedInput.Value))
                    return true;

                if (expectedInput == SkillInput.Hold_L3_Any && SkillInputs.IsL3HoldInput(receivedInput.Value))
                    return true;

                if (expectedInput == SkillInput.Hold_R3_Any && SkillInputs.IsR3HoldInput(receivedInput.Value))
                    return true;


                if (receivedInput == expectedInput)
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
