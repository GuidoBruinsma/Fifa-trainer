using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log(UnityServices.Instance.InitializeAsync());
    }

    public void FailedAttempTrack(string skillName, int totalAttempts) {

        // Get the current date (no time)
        DateTime currentDate = DateTime.Today;

        // Format the date as "yyyy-MM-dd" (Year-Month-Day)
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

    public void CompletionTimeTrackEvent(string skillName, float comletionTime) {
        CustomEvent completionTimeEvent = new CustomEvent("completionTime")
        {
            { "skillName", skillName},
            { "completionTime", comletionTime}
        };
        AnalyticsService.Instance.RecordEvent(completionTimeEvent);
        AnalyticsService.Instance.Flush();
    }
}
