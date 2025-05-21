using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the lifecycle and flow of skill move sequences during a training session.
/// Handles skill move selection based on different strategies (Normal, Adaptive, Random, Debug),
/// tracks progress, and responds to success/failure events.
/// </summary>
[RequireComponent(typeof(SkillsValidator), typeof(InputHandler))]
public class SkillMovesManager : MonoBehaviour
{
    /// <summary>
    /// Defines the mode of skill selection for the training session.
    /// </summary>
    private enum Type
    {
        Normal,
        Adaptive,
        Random,
        Debug
    }

    public static SkillMovesManager Instance { get; private set; }

    /// <summary>
    /// Returns the currently active skill.
    /// </summary>
    public static Skill CurrentSkill => Instance.currentSkill;

    [SerializeField] private Type type;

    [Header("References")]
    [SerializeField] private SkillGameSettings skillsSettings;
    private SkillsValidator sequenceValidator;
    private InputHandler inputHandler;

    [Space]
    [SerializeField] private List<Skill> skillMoves;
    [SerializeField] private Skill currentSkill;

    private int currentSequenceIndex = 0;

    /// <summary>
    /// Singleton initialization.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Initializes the manager, sets up skill list and event listeners.
    /// </summary>
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

    /// <summary>
    /// Prepares the skill list based on the selected session type (Normal, Adaptive, Random, Debug).
    /// </summary>
    private void SetupSkillsGameSettings()
    {
        if (skillsSettings == null)
        {
            Debug.LogError("No game settings attached");
            return;
        }
        if (type == Type.Normal)
        {
            if (skillsSettings.selectedSkillMoves.Count < 1)
            {
                skillMoves = new(skillsSettings.allSkillMoves);
            }
            else
            {
                skillMoves = new(skillsSettings.selectedSkillMoves);
            }
        }
        else if (type == Type.Adaptive)
        {
            skillMoves = new(SkillStatsManager.GetSortedByRateSkillList(skillsSettings.allSkillMoves));
        }
        else if (type == Type.Random)
        {
            //shuffledList = myList.OrderBy( x => Random.value ).ToList( );
            skillMoves = new(skillsSettings.allSkillMoves.OrderBy(x => Random.value).ToList());
        }
        else if (type == Type.Debug)
        {
            skillMoves = new(skillsSettings.allSkillMoves);
        }
    }

    /// <summary>
    /// Resets session progress and reloads the first skill.
    /// </summary>
    private void RestartGame()
    {
        currentSequenceIndex = 0;

        LoadCurrentSkillMove();
    }

    /// <summary>
    /// Loads and displays the current skill move and sets the input sequence.
    /// </summary>
    private void LoadCurrentSkillMove()
    {
        if (currentSequenceIndex >= skillMoves.Count)
        {
            Debug.LogWarning("No more skill moves available.");
            return;
        }

        currentSkill = skillMoves[currentSequenceIndex];
        //currentSkill.ResetStats();

        //currentSkill.SendAnalytics();
        EventManager.OnSkillChanged?.Invoke(currentSkill);

        if (currentSkill.inputSequence == null || currentSkill.inputSequence.Count == 0)
        {
            Debug.LogError($"Skill '{currentSkill.moveName}' has no input sequence!");
            return;
        }

        sequenceValidator.SetSequenceInput(currentSkill.inputSequence[0].input, currentSkill);

        inputHandler.ResetHold();
        PredictionSkill();
    }

    /// <summary>
    /// Advances to the next skill in the list and loads it.
    /// </summary>
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

    /// <summary>
    /// Predicts and shows the next skill move in the UI (if available).
    /// </summary>
    private void PredictionSkill()
    {
        UI_Manager uiManager = UI_Manager.Instance;

        int nextSkillIndex = currentSequenceIndex + 1;

        if (nextSkillIndex <= skillMoves.Count - 1)
        {
            Skill skill = skillMoves[nextSkillIndex];
            uiManager.SetNextMoveInfo(skillMoves[nextSkillIndex].inputSequence, skill.moveName);
        }
    }

    /// <summary>
    /// Handles logic when a skill move sequence is failed.
    /// Increments attempt counter and updates UI.
    /// </summary>
    private void HandleSequenceFail()
    {
        inputHandler.CancelHoldAndWaitForRelease();

        currentSkill.attempts++;
        EventManager.OnSkillChanged?.Invoke(currentSkill);
    }

    /// <summary>
    /// Handles logic when a skill move sequence is completed successfully.
    /// Increments success counter, logs stats, and loads the next move.
    /// </summary>
    private void HandleSequenceSuccess()
    {
        inputHandler.CancelHold();
        inputHandler.CancelHoldAndWaitForRelease();

        currentSkill.successes++;

        SkillChartData skillChartData = new SkillChartData
        {
            skillName = currentSkill.moveName,
            successes = currentSkill.successes,
            attempts = currentSkill.attempts,
            successRate = currentSkill.SuccessRate
        };

        GlobalDataManager.SaveData(skillChartData, isTemp: true);  // Save to temp
        GlobalDataManager.SaveData(skillChartData, isTemp: false); // Save to permanent history

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

    /// <summary>
    /// Removes all event listeners when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        EventManager.OnSequenceSuccess.RemoveListener(HandleSequenceSuccess);
        EventManager.OnSequenceFailed.RemoveListener(HandleSequenceFail);
        EventManager.OnWholeSessionFailed.RemoveListener(RestartGame);
    }
}
