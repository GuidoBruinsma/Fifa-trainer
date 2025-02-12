using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillControlIconMap", menuName = "Skill Input/Icon Mapping", order = 1)]
public class SkillControlIconMap : ScriptableObject
{
    [Header("Controller Buttons")]
    public string L1Icon;
    public string L2Icon;
    public string L3Icon;
    public string R1Icon;
    public string R2Icon;
    public string R3Icon;

    public string ButtonXIcon;
    public string ButtonTriangleIcon;
    public string ButtonCircleIcon;
    public string ButtonSquareIcon;

    [Header("Rotations")]
    public string L3_RotateIcon;
    public string R3_RotateIcon;

    [Header("Hold Actions")]
    public string L1_HoldIcon;
    public string L2_HoldIcon;
    public string R1_HoldIcon;
    public string R2_HoldIcon;

    [Header("Right Stick Directions")]
    public string RS_LeftIcon;
    public string RS_UpLeftIcon;
    public string RS_DownLeftIcon;
    public string RS_RightIcon;
    public string RS_UpRightIcon;
    public string RS_DownRightIcon;
    public string RS_UpIcon;
    public string RS_DownIcon;
    public string RS_NoneIcon;

    public string GetIconForSkillInput(SkillInput input)
    {
        return input switch
        {
            SkillInput.L1 => L1Icon,
            SkillInput.L2 => L2Icon,
            SkillInput.L3 => L3Icon,
            SkillInput.R1 => R1Icon,
            SkillInput.R2 => R2Icon,
            SkillInput.R3 => R3Icon,
            SkillInput.Button_X => ButtonXIcon,
            SkillInput.Button_Triangle => ButtonTriangleIcon,
            SkillInput.Button_Circle => ButtonCircleIcon,
            SkillInput.Button_Square => ButtonSquareIcon,
            SkillInput.L3_Rotate => L3_RotateIcon,
            SkillInput.R3_Rotate => R3_RotateIcon,
            SkillInput.L1_Hold => L1_HoldIcon,
            SkillInput.L2_Hold => L2_HoldIcon,
            SkillInput.R1_Hold => R1_HoldIcon,
            SkillInput.R2_Hold => R2_HoldIcon,
            SkillInput.RS_Left => RS_LeftIcon,
            SkillInput.RS_UpLeft => RS_UpLeftIcon,
            SkillInput.RS_DownLeft => RS_DownLeftIcon,
            SkillInput.RS_Right => RS_RightIcon,
            SkillInput.RS_UpRight => RS_UpRightIcon,
            SkillInput.RS_DownRight => RS_DownRightIcon,
            SkillInput.RS_Up => RS_UpIcon,
            SkillInput.RS_Down => RS_DownIcon,
            SkillInput.RS_None => RS_NoneIcon,
            _ => string.Empty,
        };
    }

    public string SkillInputToString(SkillInput input)
    {
        return GetIconForSkillInput(input);
    }

    public string GetSequenceDisplay(List<SkillInput> sequence, int highlightIndex = -1, bool highlight = false)
    {
        string display = "";
        for (int i = 0; i < sequence.Count; i++)
        {
            string inputStr = SkillInputToString(sequence[i]);

            if (highlight && i == highlightIndex)
            {
                inputStr = $"<color=green>{inputStr}</color>";
            }

            display += inputStr;

            if (i < sequence.Count - 1)
            {
                display += " - ";
            }
        }
        return display;
    }
}
