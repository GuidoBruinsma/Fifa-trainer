using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Validates the sequence of skill inputs entered by the player against a target skill sequence.
/// Tracks attempts, success, and timing for performance analytics.
/// </summary>
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

    /// <summary>
    /// Subscribes to input events and initializes visualization.
    /// </summary>
    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(AddInput);
        EventManager.OnMultipleInputsSent.AddListener(AddInput);
        currentSequenceInputHolder = new(currentSkill.inputSequence);
        sq.VisualizeSequence(currentSkill.inputSequence, pressedSequenceInput.Count);
    }

    /// <summary>
    /// Unsubscribes from input events on disable.
    /// </summary>
    private void OnDisable()
    {
        EventManager.OnSkillInputReceived.RemoveListener(AddInput);
        EventManager.OnMultipleInputsSent.RemoveListener(AddInput);

    }

    /// <summary>
    /// Tracks timer and fails the sequence if time runs out.
    /// </summary>
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

    /// <summary>
    /// Adds a single skill input and validates the sequence.
    /// </summary>
    /// <param name="input">The skill input received.</param>
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
        }
    }

    /// <summary>
    /// Adds multiple inputs at once and validates them against the skill sequence.
    /// </summary>
    /// <param name="input">The list of inputs received.</param>
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
            Debug.Log(elapsedTime + " " + Time.time);
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);
            
            //FIX: Just for testing. Delete later
            //if (AuthenticationManager.instance != null) 
            //AnalyticsManager.Instance.CompletionTimeTrackEvent(currentSkill.moveName, elapsedTime);

        }
    }

    /// <summary>
    /// Checks if the latest single input is valid according to the expected sequence.
    /// </summary>
    /// <returns>True if valid; otherwise false.</returns>
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

    /// <summary>
    /// Checks if any of the received inputs match the expected input at the current sequence step.
    /// </summary>
    /// <param name="receivedInputs">List of possible inputs.</param>
    /// <returns>True if valid; otherwise false.</returns>
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

    /// <summary>
    /// Handles logic for when the input sequence fails.
    /// </summary>
    private void SequenceFailed()
    {
        ResetSequence();
        EventManager.OnSequenceFailed?.Invoke();
        EventManager.OnSkillIsCompleted?.Invoke(false);
        sq.VisualizeSequence(currentSkill.inputSequence, 0);

        currentSkill.SendAnalytics();
    }

    /// <summary>
    /// Sets the current sequence inputs and skill to validate.
    /// </summary>
    /// <param name="currentSequenceInput">List of expected inputs.</param>
    /// <param name="skill">The skill associated with the input sequence.</param>
    public void SetSequenceInput(List<SkillInput> currentSequenceInput, Skill skill)
    {
        this.currentSequenceInput = new(currentSequenceInput);
        currentSkill = skill;
        timeLeftToPress = currentSkill.maxTimeBetweenInput;
    }

    /// <summary>
    /// Clears all pressed inputs to restart validation.
    /// </summary>
    public void ResetSequence() => pressedSequenceInput.Clear();
}
