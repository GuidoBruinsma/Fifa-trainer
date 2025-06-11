using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Manages globally accessible skill execution data.
/// </summary>
public static class GlobalDataManager
{
    private static readonly string historyFolder = Path.Combine(Application.persistentDataPath, "SkillHistory");
    private static readonly string tempFolder = Path.Combine(historyFolder, "temp");

    /// <summary>
    /// Creates the directory path for storing skill history data if it doesn't already exist.
    /// </summary>
    public static void CreatePath()
    {
        if (!Directory.Exists(historyFolder))
            Directory.CreateDirectory(historyFolder);

        if (!Directory.Exists(tempFolder))
            Directory.CreateDirectory(tempFolder);
    }

    public static void SaveData(SkillChartData skillData, bool isTemp = true)
    {
        CreatePath();

        string folderPath = isTemp ? tempFolder : historyFolder;
        string fileName = $"{skillData.skillName}.json";

        string filePath = Path.Combine(folderPath, fileName);

        SkillChartDataWrapper wrapper;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            wrapper = JsonUtility.FromJson<SkillChartDataWrapper>(json);
        }
        else
        {
            wrapper = new();
        }

        wrapper.history.Add(skillData);
        string updatedJson = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, updatedJson);
        Debug.Log($"Skill data saved to: {filePath}");
    }

    /// <summary>
    /// Clears all temporary skill data.
    /// 
    /// TODO: Call it on 'Continue' button
    /// </summary>
    public static void ClearTempData()
    {
        if (Directory.Exists(tempFolder))
        {
            string[] files = Directory.GetFiles(tempFolder);
            foreach (string file in files)
            {
                File.Delete(file);
            }

            Debug.Log("Temporary skill data cleared.");
        }
    }
#if UNITY_EDITOR
    /// <summary>
    /// Clears all saved skill chart data by deleting all JSON files in the history folder.
    /// </summary>
    [MenuItem("Data Menu/Clear all data")]
    public static void ClearAllData()
    {
        CreatePath();

        string[] files = Directory.GetFiles(historyFolder, "*.json");

        foreach (string file in files)
        {
            File.Delete(file);
        }

        Debug.Log("All skill chart data has been cleared.");
    }
#endif
}


/// <summary>
/// Represents a single entry of skill performance data.
/// </summary>
[Serializable]
public class SkillChartData
{
    public string skillName;

    public int attempts;
    public int successes;
    public float successRate;

    public float reactionTime;
    public float completionTime;
    public float[] timeBetweenInputs;   //????

    public string dateTime;

    public float overallPerformance;    //Compare if it's better or worse than the previous one
}

/// <summary>
/// Wrapper class used to store a list of skill chart data entries for JSON serialization.
/// </summary>
[Serializable]
public class SkillChartDataWrapper
{
    public List<SkillChartData> history = new();
}

/// <summary>
/// Represents a single log entry for a performed skill move,
/// including the player's username, skill name, time taken, and input timing.
/// 
/// Used in Logger mode
/// 
/// </summary>
[Serializable]
public class SkillLogData
{
    public string username;
    public string skillMoveName;

    public float timeSinceStart;
    public List<float> timeBetweenInputs = new();
}

/// <summary>
/// 
/// A wrapper class used to store a list of skill log entries for serialization.
/// 
/// </summary>
[Serializable]
public class SkillLogWrapper
{
    public List<SkillLogData> data = new();
}