using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages UI elements related to skill moves, timers, and move sequences.
/// Implements a singleton pattern for global access.
/// </summary>
public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private SkillControlIconMap skillMap;

    [Space]

    [Header("Time Related Section")]
    [SerializeField] private TextMeshProUGUI timeleftText;
    [SerializeField] private TextMeshProUGUI timeElapsedText;
    [SerializeField] private TextMeshProUGUI timeElapsedComlpetionText;

    [Space]

    [Header("Next Move Section")]
    [SerializeField] private TextMeshProUGUI nextMoveName;
    [SerializeField] private TextMeshProUGUI nextMoveSequence;

    [SerializeField] private TextMeshProUGUI sequenceName;
    [SerializeField] private TextMeshProUGUI sequenceText;
    
    /// <summary>
    /// Initializes the singleton instance or destroys duplicate instances.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    
    /// <summary>
    /// Updates the UI text showing the remaining time.
    /// </summary>
    /// <param name="timeLeft">Time left in seconds.</param>
    public void SetTimerText(float timeLeft)
    {
        timeleftText.text = timeLeft.ToString("0.0");
    }
    
    /// <summary>
    /// Updates the UI text showing elapsed time in mm:ss format.
    /// </summary>
    /// <param name="elapsedTime">Elapsed time in seconds.</param>
    public void SetElapsedTimeText(float elapsedTime)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f); 
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeElapsedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    /// <summary>
    /// Updates the UI text showing precise skill completion time.
    /// </summary>
    /// <param name="elapsedTime">Elapsed time in seconds.</param>
    public void SetElapsedTimeCompletion(float elapsedTime) => timeElapsedComlpetionText.text = $"Skill completion time: {elapsedTime.ToString("0.000")}";
    
    /// <summary>
    /// Sets the current skill move name and its input sequence on the UI.
    /// </summary>
    /// <param name="sequence">The move input sequence as string.</param>
    public void SetSkillMoveInfo(string sequence)
    {
        sequenceName.text = SkillMovesManager.CurrentSkill.moveName;
        sequenceText.text = sequence;
    }

    /// <summary>
    /// Displays the next move's name and input sequence on the UI.
    /// </summary>
    /// <param name="sequenceList">List of input holders representing the next move sequence.</param>
    /// <param name="moveName">Name of the next skill move.</param>
    public void SetNextMoveInfo(List<SkillInputHolder> sequenceList, string moveName)
    {
        nextMoveName.text = moveName;
        nextMoveSequence.text = skillMap.GetSequenceDisplay(sequenceList);
    }
}
