using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Manages all global game events using UnityEvent for broadcasting and subscribing.
/// </summary>
public static class EventManager
{
    /// <summary>
    /// Triggered when multiple skill inputs are sent at once.
    /// </summary>
    public static UnityEvent<List<SkillInput?>> OnMultipleInputsSent = new();

    /// <summary>
    /// Triggered when a single skill input is received.
    /// </summary>
    public static UnityEvent<SkillInput> OnSkillInputReceived = new();

    /// <summary>
    /// Triggered when a skill input sequence is successfully completed.
    /// </summary>
    public static UnityEvent OnSequenceSuccess = new();

    /// <summary>
    /// Triggered when a skill input sequence fails.
    /// </summary>
    public static UnityEvent OnSequenceFailed = new();

    /// <summary>
    /// Triggered when the entire skill session is starts.
    /// </summary>
    public static UnityEvent OnSessionStart = new();

    /// <summary>
    /// Triggered when the entire skill session is failed.
    /// </summary>
    public static UnityEvent OnSessionEnd = new();

    /// <summary>
    /// Triggered when a skill is completed, used for adaptive difficulty feedback.
    /// </summary>
    public static UnityEvent<bool> OnSkillIsCompleted = new();

    /// <summary>
    /// Triggered when the difficulty level changes.
    /// </summary>
    public static UnityEvent OnDifficultyChanged = new();

    /// <summary>
    /// Triggered when the current skill is changed.
    /// </summary>
    public static UnityEvent<Skill> OnSkillChanged = new();

    /// <summary>
    /// Triggered when skill data is ready for analysis.
    /// </summary>
    public static UnityEvent<Skill> OnAnalyzeSkillDataSent = new();

    /// <summary>
    /// Triggered when the skill session should be ended (manually or naturally).
    /// </summary>
    public static UnityEvent OnRequestSessionEnd = new();
    //NOTE: CURRENTLY NOT IN USE BUT IF IT'S NEEDED JUST SUBSCRIBE AND IT WILL BE CALLED WHEN SESSION ENDS
}
