using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SkillStatEntry
{
    public string skillName;
    public int attempts;
    public int successes;
}

[System.Serializable]
public class SkillStat
{
    public List<SkillStatEntry> allSkillsEntry = new();
}

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
        SkillStat loadedStats = JsonUtility.FromJson<SkillStat>(json);

        foreach (var saved in loadedStats.allSkillsEntry)
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
    /// Saves the current attempts/successes of all skills to file.
    /// </summary>
    public static void Save(List<Skill> skills)
    {
        SkillStat skillStats = new SkillStat();

        foreach (Skill skill in skills)
        {
            skillStats.allSkillsEntry.Add(new SkillStatEntry
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
    /// Sorts a copy of the list by success rate in ascending order (lowest to highest).
    /// </summary>
    public static List<Skill> GetSortedByRateSkillList(List<Skill> all)
    {
        List<Skill> sortedList = new(all);
        sortedList.Sort((a, b) => a.SuccessRate.CompareTo(b.SuccessRate));
        return sortedList;
    }
}
