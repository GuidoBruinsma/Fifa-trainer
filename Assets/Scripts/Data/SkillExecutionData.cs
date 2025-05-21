using System.IO;
using UnityEngine;

/// <summary>
/// Stores and manages data related to the execution of a specific FIFA skill move.
/// </summary>
[System.Serializable]
public class SkillExecutionData
{
    public string skillName;
    public float executionTime;

    public int successfulAttempts;
    public int totalAttempts;

    /// <summary>
    /// Initializes a new instance of the SkillExecutionData class with the specified skill name and execution time.
    /// </summary>
    /// <param name="skillName">The name of the skill move.</param>
    /// <param name="executionTime">The time it took to execute the skill.</param>
    public SkillExecutionData(string skillName, float executionTime)
    {
        this.skillName = skillName;
        this.executionTime = executionTime;
        this.successfulAttempts = 0;
        this.totalAttempts = 0;
    }

    /// <summary>
    /// Records an attempt at the skill and updates the success count if applicable.
    /// </summary>
    /// <param name="isSuccessful">Whether the attempt was successful.</param>
    public void AddAttempt(bool isSuccessful)
    {
        totalAttempts++;

        if (isSuccessful)
        {
            successfulAttempts++;
        }
        Debug.Log(totalAttempts);
    }

    private static readonly string path = Path.Combine(Application.persistentDataPath, "data.alibaba");

    /// <summary>
    /// Saves the current skill execution data as a JSON file to persistent storage.
    /// </summary>
    public void SaveData()
    {
        string data = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, data);
    }
}
