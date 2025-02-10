using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillsValidator : MonoBehaviour
{
    public SequenceVisualizer sq;
    public UnityAction OnSequenceSuccess;
    public UnityAction OnSequenceFailed;

    [SerializeField] private List<SkillInput> currentSequenceInput;  //The Move Input sequence
    [SerializeField] private List<SkillInput> pressedSequenceInput = new();

    private Skill currentSkill;
    private float timeLeftToPress;

    private void Start()
    {
        EventManager.OnSkillInputReceived.AddListener(AddInput);
        sq.VisualizeSequence(currentSequenceInput, pressedSequenceInput.Count);
    }

    private void OnDisable() => EventManager.OnSkillInputReceived.RemoveListener(AddInput);

    private void FixedUpdate()
    {
        if (currentSkill == null) return;

        timeLeftToPress -= Time.fixedDeltaTime;
        UI_Manager.Instance.SetTimerText(timeLeftToPress);

        if (timeLeftToPress <= 0)
        {
            ResetSequence();
            timeLeftToPress = 0;
            EventManager.OnWholeSessionFailed?.Invoke();
        }
    }

    public void AddInput(SkillInput input)
    {
        pressedSequenceInput.Add(input);

        sq.VisualizeSequence(currentSequenceInput, pressedSequenceInput.Count);

        if (!CheckValidity())
        {
            ResetSequence();
            OnSequenceFailed?.Invoke();
        }
        else if (currentSequenceInput.Count == pressedSequenceInput.Count)
        {
            OnSequenceSuccess?.Invoke();
            ResetSequence();
        }
    }

    private bool CheckValidity()
    {
        for (int i = 0; i < pressedSequenceInput.Count; i++)
        {
            if (pressedSequenceInput[i] != currentSequenceInput[i])
            {
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

    public void Restart(List<SkillInput> currentSequenceInput, Skill skill) => SetSequenceInput(currentSequenceInput, skill);
}
