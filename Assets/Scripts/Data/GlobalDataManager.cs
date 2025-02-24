public static class GlobalDataManager
{
    public static SkillExecutionData globalData { get; private set; }

    public static void SetNewData(string skillName = null, float executionTime = 0)
    {
        globalData = new(skillName, executionTime);
        globalData.SaveData();
    }
}
