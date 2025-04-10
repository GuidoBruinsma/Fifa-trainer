using System.Collections.Generic;

[System.Serializable]
public class SkillLogData
{
    public string username;
    public string skillMoveName;

    public float timeSinceStart;
    public List<float> timeBetweenInputs = new();
}

[System.Serializable]
public class SkillLogWrapper {
    public List<SkillLogData> data = new();
}
