using System.Collections.Generic;
using UnityEngine;

public class InfoLogManager : MonoBehaviour
{
    [SerializeField] private List<Skill> allSkills;
    [SerializeField] List<Skill> candidateSkills = new();

    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(ValidateCurrentInput);
        EventManager.OnMultipleInputsSent.AddListener(ValidateMultipleCurrentInputs);

        StartSequence();
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

        CheckValidity(skillInput);
        CorrectSkill();

        ResetSequece();
    }

    void ValidateMultipleCurrentInputs(List<SkillInput?> skillInput)
    {
        CheckValidityAnalog(skillInput);
        CorrectSkill();

        ResetSequece();
    }

    private void StartSequence()
    {
        candidateSkills = new List<Skill>(allSkills);
        pressedInputSequnce.Clear();
    }

    public List<SkillInput> pressedInputSequnce = new();
    private List<Skill> successful = new();


    private void CheckValidity(SkillInput currentInput)
    {
        if (candidateSkills.Count == 0)
        {
            StartSequence();
        }

        pressedInputSequnce.Add(currentInput);
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
                successful.Add(candidate);
            }
        }
    }

    private void CheckValidityAnalog(List<SkillInput?> analogInputs)
    {

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

        SkillInput analogInput = analogInputs[0].Value;
        Debug.Log($"[Analog] Received input: {analogInput}");

        pressedInputSequnce.Add(analogInput);

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
                    Debug.Log($"Correct skill {currentSkill.moveName}");
                }
            }
        }
        if (currentSkill != null) candidateSkills.Remove(currentSkill);
    }

    private void ResetSequece()
    {
        if (candidateSkills.Count == 0) //No skills available for this sequence
        {
            StartSequence();
        }
    }
}