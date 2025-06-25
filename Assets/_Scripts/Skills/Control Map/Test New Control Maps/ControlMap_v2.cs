using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillControlIconMap", menuName = "Skill Input/Icon Mapping New", order = 1)]
public class ControlMap_v2 : ScriptableObject
{
    public List<SkillInputIcon> inputIcons;

    public SkillInputIcon GetIconData(SkillInput input)
    {
        return inputIcons.Find(icon => icon.input == input);
    }
}

[System.Serializable]
public struct SkillInputIcon {
    public SkillInput input;
    public Sprite icon;
    public string fallbackText;
}