using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages adaptive feedback based on the player's skill success rate,
/// adjusting difficulty dynamically depending on performance.
/// </summary>
public class AdaptiveFeedbackManager : MonoBehaviour
{
    /*
     * Pseudo
     * 
     * keep track on how many moves has been performed
     * if divide the total moves with success moves will get the percentage of success
     * if is > 80% that means is going good and difficulty needs to increase
     * if i'ts less, = bad performance = easier
     * 
     */
    [SerializeField] private SkillGameSettings skillGameSettings;

    [SerializeField] private float difficultyScore;

    [SerializeField] private int total;
    [SerializeField] private int successful;

    [SerializeField] private int adjustmentRate;

    [SerializeField] private Skill currentSkill;
    [SerializeField] private List<Skill> skillList = new();
    
    /// <summary>
    /// Initializes skill stats and sets up event listeners.
    /// </summary>
    private void Start()
    {
        SkillStatsManager.Load(skillGameSettings.allSkillMoves);

        EventManager.OnSkillIsCompleted.AddListener(ReceivedScore);

    }

    /// <summary>
    /// Called when a skill completion result is received. Updates counters and triggers difficulty adjustment.
    /// </summary>
    /// <param name="isCompleted">Indicates whether the skill was completed successfully.</param>
    void ReceivedScore(bool isCompleted)
    {
        total++;

        if (isCompleted)
            successful++;

        if (total % adjustmentRate == 0)
        {
            AdjustDifficulty();
        }
    }

    /// <summary>
    /// Adjusts the difficulty based on the player's success rate.
    /// </summary>
    void AdjustDifficulty()
    {
        float successRate = (float)successful / total;


        if (successRate > 0.8f)
        {
            //Make it harder if the success rate is > 80%
            //more difficult moves?
            //more time to execute?
        }
        if (successRate < 0.5f)
        {
            //make it easier
        }
    }

    /// <summary>
    /// Sets the current skill from the SkillMovesManager.
    /// </summary>
    void SkillSuccessRate()
    {
        currentSkill = SkillMovesManager.CurrentSkill;
    }

    /// <summary>
    /// Saves skill stats when the application quits.
    /// </summary>
    private void OnApplicationQuit()
    {
        SkillStatsManager.Save(skillGameSettings.allSkillMoves);
    }
}
