using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles skill input validation, logging, and sequence tracking for player-performed skills.
/// </summary>
public class InfoLogManager : MonoBehaviour
{
    [SerializeField] private List<Skill> allSkills;
    [SerializeField] private List<Skill> candidateSkills = new();

    [SerializeField] private List<SkillInput> pressedInputSequnce = new();
    private List<Skill> successful = new();

    private SkillLogWrapper skillLogData = new();
    private string path;

    private float totalSkillCompletionTime;
    private List<float> _timeBetweenInputs = new();

    private float skillStartTime;
    private float lastInputTime;

    /// <summary>
    /// Initializes the path for saving skill log data.
    /// </summary>
    private void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "skill_logging_data.json");
    }

    /// <summary>
    /// Subscribes to input events and starts the initial skill sequence.
    /// </summary>
    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(ValidateCurrentInput);
        EventManager.OnMultipleInputsSent.AddListener(ValidateMultipleCurrentInputs);

        StartSequence();
    }

    /// <summary>
    /// Validates a single skill input and checks if it matches any candidate skill sequences.
    /// </summary>
    void ValidateCurrentInput(SkillInput skillInput)
    {
        if (skillInput == SkillInput.Flick_None ||
            skillInput == SkillInput.Hold_L3_None ||
            skillInput == SkillInput.Hold_None ||
            skillInput == SkillInput.Hold_R3_None ||
            skillInput == SkillInput.L2_None ||
            skillInput == SkillInput.R2_None ||
            skillInput == SkillInput.L3_None ||
            skillInput == SkillInput.R3_None)
        {
            return;
        }

        CheckValidity(skillInput);
        CorrectSkill();

        ResetSequece();
    }

    /// <summary>
    /// Validates a sequence of analog skill inputs and checks if they match any skill sequences.
    /// </summary>
    void ValidateMultipleCurrentInputs(List<SkillInput?> skillInput)
    {
        CheckValidityAnalog(skillInput);
        CorrectSkill();

        ResetSequece();
    }

    /// <summary>
    /// Initializes the skill input tracking for a new attempt.
    /// </summary>
    private void StartSequence()
    {
        candidateSkills = new List<Skill>(allSkills);
        pressedInputSequnce.Clear();
    }

    /// <summary>
    /// Tracks the time between inputs during a skill sequence.
    /// </summary>
    private void TimeTracker()
    {
        float currentTime = Time.time;

        if (pressedInputSequnce.Count == 1)
        {
            skillStartTime = currentTime;
            _timeBetweenInputs.Clear();
        }
        else
        {
            _timeBetweenInputs.Add(currentTime - lastInputTime);
        }

        lastInputTime = currentTime;
    }

    /// <summary>
    /// Filters candidate skills based on the current input to identify valid skill sequences.
    /// </summary>
    private void CheckValidity(SkillInput currentInput)
    {
        if (candidateSkills.Count == 0)
        {
            StartSequence();
        }

        pressedInputSequnce.Add(currentInput);
        TimeTracker();

        int pressedInputIndex = pressedInputSequnce.Count - 1;

        List<Skill> filteredCandidates = new List<Skill>();

        foreach (Skill candidate in candidateSkills)
        {

            bool isValidSkill = false;

            foreach (var sequence in candidate.inputSequence)
            {
                List<SkillInput> inputList = sequence.input;

                if (pressedInputIndex >= inputList.Count)
                {
                    pressedInputSequnce.Clear();
                    StartSequence();
                    continue;
                }

                if (inputList[pressedInputIndex] == currentInput)
                {
                    isValidSkill = true;
                    break;
                }
            }

            if (isValidSkill)
            {
                filteredCandidates.Add(candidate);
            }

            candidateSkills = filteredCandidates;
        }
    }

    /// <summary>
    /// Filters candidate skills based on the latest analog input to identify valid skill sequences.
    /// </summary>
    private void CheckValidityAnalog(List<SkillInput?> analogInputs)
    {
        Debug.Log("here");
        foreach (var input in analogInputs)
        {
            Debug.Log($"The gotten input {input} and input with index 0 {analogInputs[analogInputs.Count - 1].Value}, elements in the list {analogInputs.Count}");
        }
        if (analogInputs == null || analogInputs.Count == 0)
        {
            return;
        }

        if (!analogInputs[0].HasValue)
        {
            return;
        }

        if (candidateSkills.Count == 0)
        {
            StartSequence();
        }

        SkillInput analogInput = analogInputs[analogInputs.Count - 1].Value;
        Debug.Log($"[Analog] Received input: {analogInput}");

        pressedInputSequnce.Add(analogInput);

        TimeTracker();

        int pressedInputIndex = pressedInputSequnce.Count - 1;

        List<Skill> filteredCandidates = new List<Skill>();

        foreach (Skill candidate in candidateSkills)
        {
            bool isValidSkill = false;

            foreach (var sequence in candidate.inputSequence)
            {
                List<SkillInput> inputList = sequence.input;

                if (pressedInputIndex > inputList.Count - 1)
                {
                    pressedInputSequnce.Clear();
                    StartSequence();
                    continue;
                }

                if (inputList[pressedInputIndex] == analogInput)
                {
                    isValidSkill = true;
                    break;
                }
            }

            if (isValidSkill)
            {
                filteredCandidates.Add(candidate);
            }
        }

        candidateSkills = filteredCandidates;
    }

    /// <summary>
    /// Checks if the current sequence matches any known skill, logs it, and saves the result if successful.
    /// </summary>
    private void CorrectSkill()
    {
        Skill currentSkill = null;

        foreach (var candidateSkill in candidateSkills)
        {
            foreach (var candidateSkillSequence in candidateSkill.inputSequence)
            {
                var input = candidateSkillSequence.input;

                if (pressedInputSequnce.Count == input.Count)
                {
                    currentSkill = candidateSkill;
                    //Debug.Log($"Correct skill {currentSkill.moveName}");
                }
            }
        }
        if (currentSkill != null)
        {
            var logInfo = new SkillLogData
            {
                skillMoveName = currentSkill.moveName,
                username = $"Test {UnityEngine.Random.Range(0, 1000)}",
                timeSinceStart = Time.time - skillStartTime,
                timeBetweenInputs = new List<float>(_timeBetweenInputs)
            };

            skillLogData.data.Add(logInfo);
            SaveLogToJson();
            candidateSkills.Remove(currentSkill);

            Debug.Log($"Correct skill {currentSkill.moveName}");
        }
    }

    /// <summary>
    /// Saves the logged skill performance data to a JSON file.
    /// </summary>
    private void SaveLogToJson()
    {
        string json = JsonUtility.ToJson(skillLogData, true);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Resets the current skill sequence if no valid candidates remain.
    /// </summary>
    private void ResetSequece()
    {
        if (candidateSkills.Count == 0) //No skills available for this sequence
        {
            StartSequence();
        }
    }
}