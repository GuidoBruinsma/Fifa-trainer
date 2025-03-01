using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class SkillsValidator : MonoBehaviour
{
    public SequenceVisualizer sq;

    [SerializeField] private List<SkillInput> currentSequenceInput;  //The Move Input sequence
    [SerializeField] private List<SkillInputHolder> currentSequencessInput;  //The Move Input sequence

    [SerializeField] private List<SkillInput> pressedSequenceInput = new();

    private Skill currentSkill;
    private float timeLeftToPress;

    private float currentTime;
    [SerializeField] private int totalAttempts;
    int c = 0;

    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(AddInput);
        sq.VisualizeSequence(currentSequenceInput, pressedSequenceInput.Count);
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
        Debug.Log(currentSkill.inputSequence.Count);



        foreach (var input in currentSkill.inputSequence)
        {
            foreach (var item in input.input)
            {
                Debug.Log($"{item}");
            }
        }
    }

    public void AddInput(SkillInput input)
    {
        if (pressedSequenceInput.Count == 0) currentTime = Time.time;

        if (input == SkillInput.L2_None || input == SkillInput.R2_None || input == SkillInput.R3_None || input == SkillInput.None)
            return;

        pressedSequenceInput.Add(input);
        sq.VisualizeSequence(currentSequenceInput, pressedSequenceInput.Count);

        if (!CheckValidity())
        {
            // Debug.Log(input);

            totalAttempts++;
            ResetSequence();
            EventManager.OnSequenceFailed?.Invoke();
            sq.VisualizeSequence(currentSequenceInput, 0);
        }
        else if (currentSequenceInput.Count == pressedSequenceInput.Count)
        {
            EventManager.OnSequenceSuccess?.Invoke();
            sq.VisualizeSequence(currentSequenceInput, pressedSequenceInput.Count);

            float elapsedTime = (Time.time - currentTime);
            UI_Manager.Instance?.SetElapsedTimeCompletion(elapsedTime);

            ResetSequence();

            GlobalDataManager.SetNewData(currentSkill.moveName, 0.8f);
        }
    }
    //TODO: Add X or O type of moves
    private bool CheckValidity()
    {
        currentSequencessInput = new(currentSkill.inputSequence);

        for (int i = 0; i < pressedSequenceInput.Count; i++)
        {
            if (pressedSequenceInput[i] != currentSequenceInput[i])
            {

                //FIX: 
                //check if the input exists in one of the lists with valid inputs, if it's not in any of the lists
                //return false, exists in only one list, remove the rest

                c++;
                if (c > currentSequencessInput.Count)
                {

                }
                    //foreach (var item in collection)
                    //{

                    //}
                    //0
                    //    0+1
                    //    spri da chetesh 0
                    //    1+1
                    //    spri da chetesh 1
                    //    2+1
                    //    spri da chetesh 

                    return false;
                
            }
        }
            
        //TODO: ADD CORRECT MOVE LOGIC HERE

        Debug.Log("Correct button pressed. TODO: ADD CORRECT BUTTON LOGIC");
        timeLeftToPress = currentSkill.maxTimeBetweenInput;

        return true;
    }

    public void SetSequenceInput(List<SkillInput> currentSequenceInput, Skill skill)
    {
        this.currentSequenceInput = new(currentSequenceInput);
        currentSkill = skill;
        timeLeftToPress = currentSkill.maxTimeBetweenInput;
    }

    public void ResetSequence() => pressedSequenceInput.Clear();
}
