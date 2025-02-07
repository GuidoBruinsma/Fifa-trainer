using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InputHandler))]
public class SkillsValidator : MonoBehaviour
{
    public SequenceVisualizer sq;
    public UnityAction OnSequenceSuccess;
    public UnityAction OnSequenceFailed;

    [SerializeField] private List<SkillInput> currentSequenceInput;  //The Move Input sequence
    [SerializeField] private List<SkillInput> pressedSequenceInput = new();

    private InputHandler inputHandler;

    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        EventManager.OnSkillInputReceived.AddListener(AddInput);
        sq.VisualizeSequence(currentSequenceInput, pressedSequenceInput.Count);
    }

    private void OnDisable() => EventManager.OnSkillInputReceived.RemoveListener(AddInput);

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
        return true;
    }

    public void SetSequenceInput(List<SkillInput> currentSequenceInput)
    {
        this.currentSequenceInput = new(currentSequenceInput);
    }

    public void ResetSequence() => pressedSequenceInput.Clear();

}
