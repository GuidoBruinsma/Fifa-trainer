using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveChartInfo
{
    private static readonly string historyFolder = Path.Combine(Application.persistentDataPath, "SkillHistory");

    public static void CreatePath()
    {
        if (!Directory.Exists(historyFolder))
        {
            Directory.CreateDirectory(historyFolder);
        }
    }

    public static void DataToSave(Skill currentSkill)
    {

        SkillChartData skillChartData = new SkillChartData()
        {
            moveName = currentSkill.moveName,
            attempts = currentSkill.attempts,
            success = currentSkill.successes
        };

        SaveChart(skillChartData);
    }

    public static void SaveChart(SkillChartData skillData)
    {
        CreatePath();

        string filePath = Path.Combine(historyFolder, $"{skillData.moveName}.json");

        SkillChartDataWrapper wrapper;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            wrapper = JsonUtility.FromJson<SkillChartDataWrapper>(json);
        }
        else
        {
            wrapper = new SkillChartDataWrapper();
        }

        wrapper.history.Add(skillData);

        string updatedJson = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, updatedJson);
    }

    public static SkillChartDataWrapper LoadChart(Skill skillData)
    {
        string filePath = Path.Combine(historyFolder, $"{skillData.moveName}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No saved skill chart data found.");
            SkillChartData skillChartData = new SkillChartData()
            {
                moveName = null,
                attempts = 0,
                success = 0
            };
            SkillChartDataWrapper wrapper = new();
            wrapper.history.Add(skillChartData);
            return wrapper;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SkillChartDataWrapper>(json);
    }
}

[System.Serializable]
public class SkillChartDataWrapper
{
    public List<SkillChartData> history = new();
}

[System.Serializable]
public class SkillChartData
{
    public string moveName;

    public int attempts;
    public int success;
}