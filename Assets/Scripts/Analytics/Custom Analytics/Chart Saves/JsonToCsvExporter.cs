using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class JsonToCsvExporter
{
    private static readonly string historyFolder = Path.Combine(Application.persistentDataPath, "SkillHistory");
    private const string delimiter = ";";

    public static void ExportSkillHistoryToCsv()
    {
        if (!Directory.Exists(historyFolder))
        {
            Debug.LogWarning("SkillHistory folder does not exist.");
            return;
        }

        var csvLines = new List<string>();

        csvLines.Add($"SkillName{delimiter}Attempts{delimiter}Successes{delimiter}SuccessRate");

        string[] jsonFiles = Directory.GetFiles(historyFolder, "*.json");
        foreach (string file in jsonFiles)
        {
            string jsonContent = File.ReadAllText(file);

            SkillChartDataWrapper wrapper = JsonUtility.FromJson<SkillChartDataWrapper>(jsonContent);
            if (wrapper?.history == null) continue;

            foreach (var skillData in wrapper.history)
            {
                string line = $"{EscapeCsv(skillData.skillName)}{delimiter}{skillData.attempts}{delimiter}{skillData.successes}{delimiter}{skillData.successRate}";
                csvLines.Add(line);
            }
        }

        string csvString = string.Join("\n", csvLines);

        string csvPath = Path.Combine(Application.persistentDataPath, "SkillHistoryExport.csv");


        if (File.Exists(csvPath))
        {
            Debug.Log($"CSV file already exists and will be overwritten: {csvPath}");
        }

        File.WriteAllText(csvPath, csvString, Encoding.UTF8);

        Debug.Log($"Skill history exported to CSV at: {csvPath}");
    }

    private static string EscapeCsv(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        if (input.Contains(delimiter) || input.Contains("\"") || input.Contains("\n"))
        {
            input = input.Replace("\"", "\"\""); // Escape quotes
            return $"\"{input}\"";               // Wrap in quotes
        }

        return input;
    }
}

