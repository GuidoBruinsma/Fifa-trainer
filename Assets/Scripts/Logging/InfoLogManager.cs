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
        if (candidateSkills.Count == 0)
        {
            StartSequence();
        }

        SkillInput analogInput = analogInputs[0].Value;

        pressedInputSequnce.Add(analogInput);
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
        foreach (var candidate in candidateSkills)
        {
            foreach (var skill in candidate.inputSequence) { 
                var input = skill.input;

                if (input == pressedInputSequnce) {
                    Skill currentSkill = candidate;
                    StartSequence();
                    Debug.Log(currentSkill.name);
                }
            }
        }
        if (candidateSkills.Count == 1)
        {
            
        }
    }
}