using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillScrollViewData : MonoBehaviour
{
    //TODO: Get data from saved json and create the buttons and add it to the scroll view
    //when selected, send the data of that one to the chart and the rest of the windows that shows whatever information

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject contentItemPrefab;

    [SerializeField] private bool isTemp;
    [SerializeField] private LineChart lineChart;

    [Header("Is Temp References (Add only if needed)")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private TextMeshProUGUI successesText;
    [SerializeField] private TextMeshProUGUI successRateText;
    [SerializeField] private TextMeshProUGUI reactionTimeText;
    [SerializeField] private TextMeshProUGUI completionTimeText;
    [SerializeField] private TextMeshProUGUI timeBetweenInputsText;
    [SerializeField] private TextMeshProUGUI overallPerformanceText;

    private string GetFolderPath()
    {
        string basePath = Path.Combine(Application.persistentDataPath, "SkillHistory");
        if (isTemp)
            return Path.Combine(basePath, "temp");
        return basePath;
    }

    private void Start()
    {
        PopulateScrollView();
    }

    private void PopulateScrollView()
    {
        ClearExistingButtons();

        string folder = GetFolderPath();
        if (!Directory.Exists(folder)) return;

        List<string> seenSkills = new();

        string[] jsonFiles = Directory.GetFiles(folder, "*.json");

        foreach (string jsonFile in jsonFiles)
        {
            string filename = Path.GetFileNameWithoutExtension(jsonFile);

            if (seenSkills.Contains(filename)) continue;

            seenSkills.Add(filename);

            GameObject go = Instantiate(contentItemPrefab, contentParent);
            if (go.transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI t))
            {
                t.text = filename;
            }
            if (go.TryGetComponent<Button>(out var button))
            {
                string skillName = filename;
                button.onClick.AddListener(() =>
                {
                    OnSkillButtonClicked(skillName);
                });
            }
        }
    }

    private void OnSkillButtonClicked(string skillNameCopy)
    {
        if (lineChart != null)
            lineChart.LoadChartData(skillNameCopy);

        if (isTemp)
        {
            DisplayTempAnalys(skillNameCopy);
        }
    }

    private void DisplayTempAnalys(string skillNameCopy)
    {
        string filePath = Path.Combine(GetFolderPath(), $"{skillNameCopy}.json");

        if (!File.Exists(filePath))
        {
            Debug.Log(skillNameCopy);
            Debug.Log(filePath);
            Debug.LogError("The file not found");
            return;
        }

        string json = File.ReadAllText(filePath);

        SkillChartDataWrapper data = JsonUtility.FromJson<SkillChartDataWrapper>(json);

        SetUI(data);
    }

    private void SetUI(SkillChartDataWrapper data)
    {
        if (data == null) return;
        skillNameText.text = data.history[^1].skillName;
        attemptsText.text = data.history[^1].attempts.ToString();
        successesText.text = data.history[^1].successes.ToString();
        successRateText.text = data.history[^1].successRate.ToString("0.000");
        reactionTimeText.text = data.history[^1].reactionTime.ToString("0.000");
        completionTimeText.text = data.history[^1].completionTime.ToString("0.000");
        timeBetweenInputsText.text = string.Empty;

        for (int i = 0; i < data.history[^1].timeBetweenInputs.Length; i++)
        {
            timeBetweenInputsText.text += data.history[^1].timeBetweenInputs[i].ToString("0.000") + "\n";
        }

    }

    private void ClearExistingButtons()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
