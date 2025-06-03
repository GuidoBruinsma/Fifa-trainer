using System.IO;
using UnityEngine;

public class EndSessionAnalysis : MonoBehaviour
{

    private void Start()
    {
        EventManager.OnAnalyzeSkillDataSent.AddListener(SaveCurrentSkillData);
    }

    public void CreatePath()
    {
        string historyFolder = Path.Combine(Application.persistentDataPath, "TempSkillHistory");

        if (!Directory.Exists(historyFolder))
        {
            Directory.CreateDirectory(historyFolder);
        }
    }

    private void SaveCurrentSkillData(Skill skill)
    {
        CreatePath();
        //skill move completion time, time between button presses, avg time between buttons
    }
}
