using System.Collections.Generic;
using UnityEngine;

public class SkillsValidator : MonoBehaviour
{
    //TODO: Add check if last skill has been completed

    public SequenceVisualizer sq;

    [SerializeField] private List<SkillInput> currentSequenceInput;
    [SerializeField] private List<SkillInputHolder> currentSequenceInputHolder;

    [SerializeField] private List<SkillInput?> pressedSequenceInput = new();

    private Skill currentSkill;
    private float timeLeftToPress;

    [SerializeField] private float currentTime;
    [SerializeField] private int totalAttempts;

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

    //TODO: Fix elapsed time for skill completion. it returns 0 every time
    public void AddInput(SkillInput input)
    {
        if (pressedSequenceInput.Count == 0) currentTime = Time.time;

        if (input == (SkillInput.Flick_None) ||
          input == (SkillInput.L2_None) ||
          input == (SkillInput.R2_None) ||
          input == (SkillInput.L3_None) ||
          input == (SkillInput.R3_None))
        {
            return;
        }
        Debug.Log(input);
        pressedSequenceInput.Add(input);
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);

        if (!CheckValidity())
        {
            SequenceFailed();
            if (input != SkillInput.None && input != SkillInput.Flick_None && input != SkillInput.Hold_L3_None && input != SkillInput.Hold_None &&
                input != SkillInput.Hold_R3_None && input != SkillInput.L2_None && input != SkillInput.R2_None && input != SkillInput.L3_None && input != SkillInput.R3_None)
            {
                totalAttempts++;

                if (AuthenticationManager.instance != null)
                {  //FIX: Just for testing. Delete later
                    //AnalyticsManager.Instance.FailedAttempTrack(currentSkill.moveName, totalAttempts);
                }
            }
        }
        else if (currentSequenceInput.Count == pressedSequenceInput.Count)
        {
            ResetSequence();
            EventManager.OnSequenceSuccess?.Invoke();
            EventManager.OnSkillIsCompleted?.Invoke(true);

            currentSequenceInputHolder = new(currentSkill.inputSequence);
            sq.VisualizeSequence(currentSkill.inputSequence, 0);

            float elapsedTime = (Time.time - currentTime);
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);

            if (AuthenticationManager.instance != null)
            {  //FIX: Just for testing. Delete later
                //AnalyticsManager.Instance.CompletionTimeTrackEvent(currentSkill.moveName, elapsedTime);
            }
            GlobalDataManager.SetNewData(currentSkill.moveName, 0.8f);
        }
    }

    public void AddInput(List<SkillInput?> input)
    {
        if (pressedSequenceInput.Count == 0) currentTime = Time.time;

        if (input.Contains(SkillInput.Flick_None) ||
            input.Contains(SkillInput.Hold_L3_None) ||
            input.Contains(SkillInput.Hold_None) ||
            input.Contains(SkillInput.Hold_R3_None) ||
            input.Contains(SkillInput.L2_None) ||
            input.Contains(SkillInput.R2_None) ||
            input.Contains(SkillInput.L3_None) ||
            input.Contains(SkillInput.R3_None))
        {
            return;
        }
        Debug.Log(input[0]);
        pressedSequenceInput.Add(input[0]);
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);

        if (!CheckValidity(input))
        {
            SequenceFailed();

            totalAttempts++;

            if (AuthenticationManager.instance != null)
            { //FIX: Just for testing. Delete later
                //AnalyticsManager.Instance.FailedAttempTrack(currentSkill.moveName, totalAttempts);
            }
        }
        else if (currentSequenceInput.Count == pressedSequenceInput.Count)
        {
            ResetSequence();
            EventManager.OnSequenceSuccess?.Invoke();
            EventManager.OnSkillIsCompleted?.Invoke(true);

            currentSequenceInputHolder = new(currentSkill.inputSequence);
            sq.VisualizeSequence(currentSkill.inputSequence, 0);

            float elapsedTime = (Time.time - currentTime);
            Debug.Log(elapsedTime + " " + Time.time );
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);

            if (AuthenticationManager.instance != null) //FIX: Just for testing. Delete later
                //AnalyticsManager.Instance.CompletionTimeTrackEvent(currentSkill.moveName, elapsedTime);

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
        {
            return false;
        }

        foreach (var inputHolder in currentSequenceInputHolder)
        {
            var expectedInput = inputHolder.input[pressedInputIndex];

            foreach (var receivedInput in receivedInputs)
            {
                if (receivedInput == null) continue;


                if (expectedInput == SkillInput.L3_Any && SkillInputs.IsL3Input(receivedInput.Value))
                {
                    return true;
                }

                if (expectedInput == SkillInput.R3_Any && SkillInputs.IsR3Input(receivedInput.Value))
                {
                    return true;
                }

                if (expectedInput == SkillInput.Hold_L3_Any && SkillInputs.IsL3HoldInput(receivedInput.Value))
                {
                    return true;
                }

                if (expectedInput == SkillInput.Hold_R3_Any && SkillInputs.IsR3HoldInput(receivedInput.Value))
                {
                    return true;
                }

                if (receivedInput == expectedInput)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SequenceFailed()
    {
        ResetSequence();
        EventManager.OnSequenceFailed?.Invoke();
        EventManager.OnSkillIsCompleted?.Invoke(false);
        sq.VisualizeSequence(currentSkill.inputSequence, 0);

        currentSkill.SendAnalytics();
    }

    public void SetSequenceInput(List<SkillInput> currentSequenceInput, Skill skill)
    {
        this.currentSequenceInput = new(currentSequenceInput);
        currentSkill = skill;
        timeLeftToPress = currentSkill.maxTimeBetweenInput;
    }

    public void ResetSequence() => pressedSequenceInput.Clear();
}
