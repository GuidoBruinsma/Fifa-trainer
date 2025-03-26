using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkillsValidator), typeof(InputHandler))]
public class SkillMovesManager : MonoBehaviour
{
    public static SkillMovesManager Instance { get; private set; }

    public static Skill CurrentSkill => Instance.currentSkill;

    [Header("References")]
    [SerializeField] private SkillsValidator sequenceValidator;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private SkillGameSettings skillsSettings;

    [Space]
    [SerializeField] private List<Skill> skillMoves;
    [SerializeField] private Skill currentSkill;

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
        SetupSkillsGameSettings();
        if (skillMoves.Count <= 0) return;

        sequenceValidator = GetComponent<SkillsValidator>();
        inputHandler = GetComponent<InputHandler>();

        EventManager.OnSequenceSuccess.AddListener(HandleSequenceSuccess);
        EventManager.OnSequenceFailed.AddListener(HandleSequenceFail);

        EventManager.OnWholeSessionFailed.AddListener(RestartGame);

        LoadCurrentSkillMove();
    }

    private void SetupSkillsGameSettings () {
        if (skillsSettings == null) {
            Debug.LogError("No game settings attached");
            return;
        }

        if (skillsSettings.selectedSkillMoves.Count < 1)
        {
            skillMoves = new(skillsSettings.allSkillMoves);
        }
        else {
            skillMoves = new(skillsSettings.selectedSkillMoves);
        }
    }

    private void RestartGame()
    {
        currentSequenceIndex = 0;

        LoadCurrentSkillMove();
    }

    private void LoadCurrentSkillMove()
    {
        if (currentSequenceIndex >= skillMoves.Count)
        {
            Debug.LogWarning("No more skill moves available.");
            return;
        }

        currentSkill = skillMoves[currentSequenceIndex];

        if (currentSkill.inputSequence == null || currentSkill.inputSequence.Count == 0)
        {
            Debug.LogError($"Skill '{currentSkill.moveName}' has no input sequence!");
            return;
        }

        sequenceValidator.SetSequenceInput(currentSkill.inputSequence[0].input, currentSkill);

        inputHandler.ResetHold();
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
