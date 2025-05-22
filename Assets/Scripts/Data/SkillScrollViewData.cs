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
        lineChart.LoadChartData(skillNameCopy);
    }



    private void ClearExistingButtons()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
