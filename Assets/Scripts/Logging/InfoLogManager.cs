using System.Collections.Generic;
using UnityEngine;

public class InfoLogManager : MonoBehaviour
{
    [SerializeField] private List<Skill> allSkills;
    private InputHandler inputHandler;
    [SerializeField] List<Skill> candidateSkills = new();

    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        EventManager.OnSkillInputReceived.AddListener(ValidateCurrentInput);
        EventManager.OnMultipleInputsSent.AddListener(ValidateMultipleCurrentInputs);
    }

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
        //indexPosition = 0;
        CheckValidity(skillInput);
        CorrectSkill();
    }

    void ValidateMultipleCurrentInputs(List<SkillInput?> skillInput)
    {
        CheckValidityAnalog(skillInput);
        CorrectSkill();
    }

    private void StartSequence()
    {
        candidateSkills = new List<Skill>(allSkills);
        pressedInputSequnce.Clear();
    }

    public List<SkillInput> pressedInputSequnce = new();
    private List<Skill> successful = new();

    //TODO: Make a system that detects which skill is about to be performed and if any of the skills is performed, display it
    private void CheckValidity(SkillInput currentInput)
    {
        Debug.Log($"Current Input: {currentInput}");

        if (candidateSkills.Count == 0)
        {
            Debug.Log("No candidate skills, restarting sequence.");
            StartSequence();
        }

        pressedInputSequnce.Add(currentInput);
        int pressedInputIndex = pressedInputSequnce.Count - 1;
        Debug.Log($"Pressed Input Index: {pressedInputIndex}");

        List<Skill> filteredCandidates = new List<Skill>();

        foreach (Skill candidate in candidateSkills) // Iterate safely without modifying list
        {
            Debug.Log($"Checking skill: {candidate.moveName}");

            bool isValidSkill = false;

            foreach (var sequence in candidate.inputSequence) // Check all possible sequences
            {
                List<SkillInput> inputList = sequence.input;
                Debug.Log($"Skill {candidate.moveName} sequence length: {inputList.Count}");

                // Ensure the skill sequence is long enough for the current input
                if (pressedInputIndex >= inputList.Count)
                {
                    Debug.Log($"Skill {candidate.moveName} ignored: Not enough inputs.");
                    pressedInputSequnce.Clear();
                    continue;
                }

                // Check if the current input matches the expected input in the sequence
                if (inputList[pressedInputIndex] == currentInput)
                {
                    Debug.Log($"Skill {candidate.moveName} is still valid.");
                    isValidSkill = true;
                    break; // If at least one sequence matches, keep the skill
                }
            }

            if (isValidSkill)
            {
                successful.Add(candidate);
            }
            else
            {
                Debug.Log($"Skill {candidate.moveName} removed.");
            }
        }
    }

    private void CheckValidityAnalog(List<SkillInput?> analogInputs)
    {
        // For consistency, log the received analog inputs.
        Debug.Log($"[Analog] Received Inputs: {string.Join(", ", analogInputs)}");

        // Restart the candidate skills if none exist.
        if (candidateSkills.Count == 0)
        {
            Debug.Log("[Analog] No candidate skills, restarting sequence.");
            StartSequence();
        }

        SkillInput analogInput = analogInputs[0].Value;

        pressedInputSequnce.Add(analogInput);
        int pressedInputIndex = pressedInputSequnce.Count - 1;
        List<Skill> filteredCandidates = new List<Skill>();

        foreach (Skill candidate in candidateSkills)
        {
            Debug.Log($"[Analog] Checking skill: {candidate.moveName}");
            bool isValidSkill = false;

            foreach (var sequence in candidate.inputSequence)
            {
                List<SkillInput> inputList = sequence.input;

                if (pressedInputIndex >= inputList.Count)
                {
                    pressedInputSequnce.Clear();
                    continue;
                }

                if (inputList[pressedInputIndex] == analogInput)
                {
                    isValidSkill = true;
                    break; // If one sequence variant matches, we keep the candidate.
                }
            }

            if (isValidSkill)
            {
                filteredCandidates.Add(candidate);
            }
            else
            {
                Debug.Log($"[Analog] Skill {candidate.moveName} removed.");
            }
        }

        Debug.Log($"[Analog] Remaining candidate skills: {filteredCandidates.Count}");

        candidateSkills = filteredCandidates;
    }

    private void CorrectSkill()
    {
        if (candidateSkills.Count == 1)
        {
            Skill skill = candidateSkills[0];
            StartSequence();
            Debug.Log(skill.name);
        }
    }
}