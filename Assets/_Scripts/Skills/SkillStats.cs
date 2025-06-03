using System.Collections.Generic;
using System.IO;
using UnityEngine;


/// <summary>
/// Handles saving and loading of skill statistics to a JSON file.
/// </summary>
public static class SkillStatsManager
{
    private static readonly string savePath = Application.persistentDataPath + "/skillStats.json";

    /// <summary>
    /// Loads saved attempts/successes from file into the current Skill objects.
    /// If no file exists, creates a new one based on the current list.
    /// </summary>
    public static void Load(List<Skill> skills)
    {
        if (!File.Exists(savePath))
        {
            Save(skills);
            return;
        }

        string json = File.ReadAllText(savePath);
        SkillChartDataWrapper loadedStats = JsonUtility.FromJson<SkillChartDataWrapper>(json);

        foreach (var saved in loadedStats.history)
        {
            var match = skills.Find(s => s.moveName == saved.skillName);
            if (match != null)
            {
                match.attempts = saved.attempts;
                match.successes = saved.successes;
            }
        }
    }

    /// <summary>
    /// Saves the current statistics of all skills to disk.
    /// </summary>
    public static void Save(List<Skill> skills)
    {
        SkillChartDataWrapper skillStats = new SkillChartDataWrapper();

        foreach (Skill skill in skills)
        {
            skillStats.history.Add(new SkillChartData
            {
                skillName = skill.moveName,
                attempts = skill.attempts,
                successes = skill.successes
            });
        }

        string json = JsonUtility.ToJson(skillStats, true);
        File.WriteAllText(savePath, json);
    }

    /// <summary>
    /// Returns a copy of the skill list sorted by success rate (ascending).
    /// </summary>
    public static List<Skill> GetSortedByRateSkillList(List<Skill> all)
    {
        List<Skill> sortedList = new(all);
        sortedList.Sort((a, b) => a.SuccessRate.CompareTo(b.SuccessRate));
        return sortedList;
    }
}
