using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkillsValidator), typeof(InputHandler))]
public class SkillMovesManager : MonoBehaviour
{
    public static SkillMovesManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private SkillsValidator sequenceValidator;
    [SerializeField] private InputHandler inputHandler;
    [Space]
    [SerializeField] private List<Skill> skillMoves;
    [SerializeField] private Skill currentSkill;

    public static Skill CurrentSkill => Instance.currentSkill;

    private int currentSequenceIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (skillMoves.Count <= 0) return;

        sequenceValidator = GetComponent<SkillsValidator>();
        inputHandler = GetComponent<InputHandler>();

        EventManager.OnSequenceSuccess.AddListener(HandleSequenceSuccess);
        EventManager.OnSequenceFailed.AddListener(HandleSequenceFail);

        EventManager.OnWholeSessionFailed.AddListener(RestartGame);

        LoadCurrentSkillMove();
    }
    
    private void RestartGame()
    {
        currentSequenceIndex = 0;
        //GlobalDataManager.IncreaseAttempts();
        LoadCurrentSkillMove();
    }

    private void LoadCurrentSkillMove()
    {
        currentSkill = skillMoves[currentSequenceIndex];

        sequenceValidator.SetSequenceInput(currentSkill.inputSequence, currentSkill);
        inputHandler.ResetHold();

        sequenceValidator.sq.VisualizeSequence(currentSkill.inputSequence, 0);
        PredictionSkill();
    }

    private void SetCurrentSkillMove()
    {
        if (currentSequenceIndex < skillMoves.Count - 1)
        {
            Debug.Log("move completed!");

            currentSequenceIndex++;
            LoadCurrentSkillMove();
        }
        else
        {
            Debug.Log("All skill moves completed!");
        }
    }

    private void PredictionSkill() {
        UI_Manager uiManager = UI_Manager.Instance;

        int nextSkillIndex = currentSequenceIndex + 1;

        if (nextSkillIndex <= skillMoves.Count - 1)
        {
            Skill skill = skillMoves[nextSkillIndex];
            uiManager.SetNextMoveInfo(skillMoves[nextSkillIndex].inputSequence, skill.moveName);
        }
    }

    private void HandleSequenceFail()
    {
        inputHandler.CancelHoldAndWaitForRelease();
    }

    private void HandleSequenceSuccess()
    {
        inputHandler.CancelHold();
        inputHandler.CancelHoldAndWaitForRelease();

        if (currentSequenceIndex < skillMoves.Count - 1)
        {
            SetCurrentSkillMove();
        }
        else
        {
            sequenceValidator.ResetSequence();
            Debug.Log("All skill moves completed!");
        }
    }



    private void OnDisable()
    {
        EventManager.OnSequenceSuccess.RemoveListener(HandleSequenceSuccess);
        EventManager.OnSequenceFailed.RemoveListener(HandleSequenceFail);
        EventManager.OnWholeSessionFailed.RemoveListener(RestartGame);
    }
}
