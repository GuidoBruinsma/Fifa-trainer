using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

/// <summary>
/// Manages custom analytics events using Unity's Analytics and Core services.
/// Tracks user interactions like failed attempts and completion times for skills.
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    
    /// <summary>
    /// Singleton instance of the AnalyticsManager.
    /// </summary>
    public static AnalyticsManager Instance { get; private set; }
   
    /// <summary>
    /// Initializes Unity Analytics and sets up the singleton instance.
    /// Starts data collection and begins the async initialization of Unity Services.
    /// </summary>
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log(UnityServices.Instance.InitializeAsync());
    }

    /// <summary>
    /// Tracks a failed skill attempt event and logs it with the skill name, date, and number of failed attempts.
    /// </summary>
    /// <param name="skillName">The name of the skill being attempted.</param>
    /// <param name="totalAttempts">Total number of failed attempts.</param>
    public void FailedAttemptTrack(string skillName, int totalAttempts)
    {

        DateTime currentDate = DateTime.Today;

        string dateString = currentDate.ToString("dd-MM-yyyy");

        CustomEvent failsTrackEvent = new CustomEvent("failedAttemp")
        {
            { "skillName", skillName},
            { "date_time", dateString},
            { "failedAttempsCount", totalAttempts}
        };

        AnalyticsService.Instance.RecordEvent(failsTrackEvent);
        AnalyticsService.Instance.Flush();
    }


    /// <summary>
    /// Tracks the completion time of a skill and logs it with the skill name and duration.
    /// </summary>
    /// <param name="skillName">The name of the completed skill.</param>
    /// <param name="comletionTime">The time taken to complete the skill.</param>
    public void CompletionTimeTrackEvent(string skillName, float comletionTime)
    {
        CustomEvent completionTimeEvent = new CustomEvent("completionTime")
        {
            { "skillName", skillName},
            { "completionTime", comletionTime}
        };
        AnalyticsService.Instance.RecordEvent(completionTimeEvent);
        AnalyticsService.Instance.Flush();
    }
}
