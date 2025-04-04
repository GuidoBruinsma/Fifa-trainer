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
    }

    public void FailedAttempTrack(int totalAttempts) {
        CustomEvent failsTrackEvent = new CustomEvent("failedAttemp")
        {
            { "failedAttempsCount", totalAttempts}
        };
        AnalyticsService.Instance.RecordEvent(failsTrackEvent);
        AnalyticsService.Instance.Flush();
        Debug.Log(failsTrackEvent);
    }



}
