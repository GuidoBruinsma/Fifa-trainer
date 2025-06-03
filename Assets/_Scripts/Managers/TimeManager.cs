using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
    private static float sessionTime;
    private static bool sessionActive;

    private static float sessionStartTime;
    private static float sessionEndTime;

    private static float reactionStartTime;
    private static float reactionTime;
    private static bool reactionActive;

    private static float completionTime;
    private static float completionTimeStart;

    private static float lastInputTime = 0f;
    private static List<float> timeBetweenInputs = new();

    public static void StartSession()
    {
        sessionTime = 0f;
        sessionStartTime = Time.time;
        sessionActive = true;
    }

    public static void EndSession()
    {
        sessionEndTime = Time.time;
        sessionTime = sessionEndTime - sessionStartTime;
        sessionActive = false;
    }

    public static void UpdateSessionTime()
    {
        if (sessionActive)
            sessionTime += Time.deltaTime;
    }

    public static void StartReactionTime()
    {
        reactionActive = true;
        reactionStartTime = Time.time;
        reactionTime = 0;
    }

    public static void ReactionTimeCompleted()
    {
        reactionTime = Time.time - reactionStartTime;
        reactionActive = false;
    }

    public static void RegisterCompletionTime()
    {
        completionTimeStart = 0;
        completionTimeStart = Time.time;
    }

    public static void CompletionTimeCompleted()
    {
        completionTime = Time.time - completionTimeStart;
    }

    public static void RegisterInputTime(int currentInputCount)
    {
        float currentTime = Time.time;

        if (currentInputCount == 1)
        {
            lastInputTime = currentTime;
            timeBetweenInputs.Clear();
        }
        else
        {
            Debug.Log($"add {currentTime - lastInputTime}");
            timeBetweenInputs.Add(currentTime - lastInputTime);
            lastInputTime = currentTime;
        }
    }

    public static float GetCompletionTime() => completionTime;
    public static float GetReactionTime() => reactionTime;
    public static List<float> GetTimeBetweenInputs()
    {
        Debug.Log(timeBetweenInputs.Count);
        return timeBetweenInputs;
    }
    public static float GetSessionDuration() => sessionTime;
    public static bool GetReactionActive() => reactionActive;
    public static bool IsSessionActive() => sessionActive;
}
