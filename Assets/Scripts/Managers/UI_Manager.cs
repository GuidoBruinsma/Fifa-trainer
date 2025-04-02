using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [SerializeField] private TextMeshProUGUI sequenceText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetTimerText(float timeLeft)
    {
        timeleftText.text = timeLeft.ToString("0.0");
    }

    public void SetElapsedTimeText(float elapsedTime)
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f); 
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeElapsedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetElapsedTimeCompletion(float elapsedTime) => timeElapsedComlpetionText.text = $"Skill completion time: {elapsedTime.ToString("0.000")}";

    public void SetSkillMoveInfo(string sequence) => sequenceText.text = sequence;

    public void SetNextMoveInfo(List<SkillInputHolder> sequenceList, string moveName)
    {
        nextMoveName.text = moveName;
        nextMoveSequence.text = skillMap.GetSequenceDisplay(sequenceList);
    }
}
