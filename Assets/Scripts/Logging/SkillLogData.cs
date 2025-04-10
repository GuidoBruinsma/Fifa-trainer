using System.Collections.Generic;

[System.Serializable]
public class SkillLogData
{
    public string username;
    public string skillMoveName;
}

[System.Serializable]
public class SkillLogWrapper {
    public List<SkillLogData> data = new();
}
