using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles saving, loading, and clearing of skill chart data as JSON files.
/// </summary>
public static class ChartInfo
{
    private static readonly string historyFolder = Path.Combine(Application.persistentDataPath, "SkillHistory");

    /// <summary>
    /// Loads the skill chart history for the specified skill from a JSON file.
    /// If the file does not exist, returns a wrapper with a default entry.
    /// </summary>
    /// <param name="skillData">The skill for which history is to be loaded.</param>
    /// <returns>A <see cref="SkillChartDataWrapper"/> containing the skill history.</returns>
    public static SkillChartDataWrapper LoadAllTimeChart(Skill skillData)
    {
        string filePath = Path.Combine(historyFolder, $"{skillData.moveName}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No saved skill chart data found.");
            SkillChartData skillChartData = new SkillChartData()
            {
                skillName = null,
                attempts = 0,
                successes = 0
            };
            SkillChartDataWrapper wrapper = new();
            wrapper.history.Add(skillChartData);
            return wrapper;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SkillChartDataWrapper>(json);
    }

    public static SkillChartDataWrapper LoadTempChart(Skill skillData)
    {
        string filePath = Path.Combine(historyFolder, $"{skillData.moveName}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No saved skill chart data found.");
            SkillChartData skillChartData = new SkillChartData()
            {
                skillName = null,
                attempts = 0,
                successes = 0
            };
            SkillChartDataWrapper wrapper = new();
            wrapper.history.Add(skillChartData);
            return wrapper;
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SkillChartDataWrapper>(json);
    }
}
