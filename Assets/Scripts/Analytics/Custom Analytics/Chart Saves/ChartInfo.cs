using System.IO;
using System.Threading.Tasks;
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
    public static async Task<SkillChartDataWrapper> LoadAllTimeChartAsync(string filename)
    {
        string filePath = Path.Combine(historyFolder, $"{filename}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No saved skill chart data found.");
            return new SkillChartDataWrapper
            {
                history = { new SkillChartData
                {
                    skillName = null,
                    attempts = 0,
                    successes = 0
                }}
            };
        }

        try
        {
            // Read the file off the main thread
            string json = await Task.Run(() => File.ReadAllText(filePath));

            // Parse on the main thread (or off, depending on Unity version behavior)
            return JsonUtility.FromJson<SkillChartDataWrapper>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load chart data: {ex.Message}");
            return null;
        }
    }
}
