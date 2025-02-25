using System.IO;
using UnityEngine;

[System.Serializable]
public class SkillExecutionData
{
    public string skillName;
    public float executionTime;

    public int successfulAttempts;
    public int totalAttempts;

    public SkillExecutionData(string skillName, float executionTime)
    {
        this.skillName = skillName;
        this.executionTime = executionTime;
        this.successfulAttempts = 0;
        this.totalAttempts = 0;
    }

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

    //TODO: Later, instead of local, store it online and make a leaderboard
    public void SaveData()
    {
        string data = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, data);
    }
}
