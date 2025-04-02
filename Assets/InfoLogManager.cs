using System.Collections.Generic;
using UnityEngine;

public class InfoLogManager : MonoBehaviour
{
    [SerializeField] private List<Skill> allSkills;
    private InputHandler inputHandler;
    private List<SkillInput> pressedSequenceInput = new();
    [SerializeField] private int indexPosition = 0;
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
        Debug.Log(CheckValidity(skillInput));
    }

    void ValidateMultipleCurrentInputs(List<SkillInput?> skillInput)
    {
        Debug.Log(skillInput);
    }

    private void StartSequence()
    {
        candidateSkills = new List<Skill>(allSkills);
        indexPosition = 0;
    }

    private List<SkillInput> pressedInput = new();
    public List<SkillInput> successful = new();

    //TODO: Make a system that detects which skill is about to be performed and if any of the skills is performed, display it
    private List<Skill> CheckValidity(SkillInput currentInput)
    {
        if (candidateSkills.Count == 0)
        {
            StartSequence();
        }
        pressedInput.Add(currentInput);

        int pressedInputIndex = pressedInput.Count - 1;

        List<Skill> filteredCandidates = new List<Skill>();

        for (int i = candidateSkills.Count - 1; i >= 0; i--)
        {
            Skill candidate = candidateSkills[i];
            foreach (var skill in candidate.inputSequence)
            {
                var inputList = skill.input;
                if (currentInput == inputList[pressedInputIndex])
                {
                    successful.Add(currentInput);
                }
                else { 
                    candidateSkills.RemoveAt(i);
                    continue;
                }
            }
        }
        return candidateSkills;
    }
}
//int pressedInputIndex = pressedSequenceInput.Count - 1;

//SkillInput? pressedInput = pressedSequenceInput[pressedInputIndex];

//foreach (var inputHolder in currentSequenceInputHolder)
//{
//    var inputList = inputHolder.input;

//    if (inputList[pressedInputIndex] == SkillInput.L3_Any && SkillInputs.IsL3Input(pressedInput))
//    {
//        return true;
//    }
//    else if (inputList[pressedInputIndex] == SkillInput.R3_Any && SkillInputs.IsR3Input(pressedInput))
//    {
//        return true;
//    }
//    else if (pressedInput == inputList[pressedInputIndex])
//    {
//        return true;
//    }
//}

//return false;