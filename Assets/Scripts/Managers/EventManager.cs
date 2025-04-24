using System.Collections.Generic;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<List<SkillInput?>> OnMultipleInputsSent = new();
    public static UnityEvent<SkillInput> OnSkillInputReceived = new();
    public static UnityEvent<SkillInput> OnSkillInputReceivedFromStick = new();

    public static UnityEvent OnSequenceSuccess = new();

    public static UnityEvent OnSequenceFailed = new();

    public static UnityEvent OnWholeSessionFailed = new();

    //Adaptive difficulty feedback events here
    public static UnityEvent<bool> OnSkillIsCompleted = new();

    public static UnityEvent OnDifficultyChanged = new();

    public static UnityEvent<Skill> OnSkillChanged = new();

    //Analyze
    //public static UnityEvent<Skill> OnAnalyzeSkillDataSent = new();
}
