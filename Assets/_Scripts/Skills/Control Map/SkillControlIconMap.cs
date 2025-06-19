using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that maps skill input enums to corresponding icon strings.
/// Used for displaying controller button icons and directional inputs in the UI.
/// </summary>
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

    public string ButtonXHoldIcon;
    public string ButtonTriangleHoldIcon;
    public string ButtonCircleHoldIcon;
    public string ButtonSquareHoldIcon;

    public string L3Any;
    public string R3Any;

    [Header("Rotations")]
    public string L3_RotateIcon;
    public string R3_RotateIcon;

    [Header("Hold Actions")]
    public string L1_HoldIcon;
    public string L2_HoldIcon;
    public string R1_HoldIcon;
    public string R2_HoldIcon;

    [Header("Right Stick Directions")]
    public string R3_LeftIcon;
    public string R3_UpLeftIcon;
    public string R3_DownLeftIcon;
    public string R3_RightIcon;
    public string R3_UpRightIcon;
    public string R3_DownRightIcon;
    public string R3_UpIcon;
    public string R3_DownIcon;
    public string R3_NoneIcon;

    [Header("Left Stick Directions")]
    public string L3_LeftIcon;
    public string L3_UpLeftIcon;
    public string L3_DownLeftIcon;
    public string L3_RightIcon;
    public string L3_UpRightIcon;
    public string L3_DownRightIcon;
    public string L3_UpIcon;
    public string L3_DownIcon;

    // Add icons for new R3 and L3 directions
    [Header("R3 Directions")]
    public string R3_RightToUpIcon;
    public string R3_RightToDownIcon;
    public string R3_UpToRightIcon;
    public string R3_UpToLeftIcon;
    public string R3_DownToRightIcon;
    public string R3_DownToLeftIcon;
    public string R3_LeftToUpIcon;
    public string R3_LeftToDownIcon;
    public string R3_LeftToDownToLeftIcon;
    public string R3_LeftToUpToLeftIcon;

    // Hold Right Stick Directions
    public string Hold_R3_LeftIcon;
    public string Hold_R3_UpLeftIcon;
    public string Hold_R3_DownLeftIcon;
    public string Hold_R3_RightIcon;
    public string Hold_R3_UpRightIcon;
    public string Hold_R3_DownRightIcon;
    public string Hold_R3_UpIcon;
    public string Hold_R3_DownIcon;
    public string Hold_R3_NoneIcon;

    // Hold L3 Directions
    public string Hold_L3_LeftIcon;
    public string Hold_L3_RightIcon;
    public string Hold_L3_UpIcon;
    public string Hold_L3_DownIcon;

    public string Hold_L3_AnyIcon;
    public string Hold_R3_AnyIcon;

    public string Hold_L3_UpLeftIcon;
    public string Hold_L3_DownLeftIcon;
    public string Hold_L3_UpRightIcon;
    public string Hold_L3_DownRightIcon;

    public string L3_RightToUpIcon;
    public string L3_RightToDownIcon;
    public string L3_UpToRightIcon;
    public string L3_UpToLeftIcon;
    public string L3_DownToRightIcon;
    public string L3_DownToLeftIcon;
    public string L3_LeftToUpIcon;
    public string L3_LeftToDownIcon;
    public string L3_LeftToDownToLeftIcon;
    public string L3_LeftToUpToLeftIcon;
    public string L3_DownToRightToDown;
    public string L3_RightToDownToRight;

    /// <summary>
    /// Returns the icon string associated with the given SkillInput enum.
    /// Uses a switch expression for efficient mapping.
    /// </summary>
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

            SkillInput.L3_Any => L3Any,
            SkillInput.R3_Any => R3Any,

            SkillInput.R3_Left => R3_LeftIcon,
            SkillInput.R3_UpLeft => R3_UpLeftIcon,
            SkillInput.R3_DownLeft => R3_DownLeftIcon,
            SkillInput.R3_Right => R3_RightIcon,
            SkillInput.R3_UpRight => R3_UpRightIcon,
            SkillInput.R3_DownRight => R3_DownRightIcon,
            SkillInput.R3_Up => R3_UpIcon,
            SkillInput.R3_Down => R3_DownIcon,

            SkillInput.L3_Left => L3_LeftIcon,
            SkillInput.L3_UpLeft => L3_UpLeftIcon,
            SkillInput.L3_DownLeft => L3_DownLeftIcon,
            SkillInput.L3_Right => L3_RightIcon,
            SkillInput.L3_UpRight => L3_UpRightIcon,
            SkillInput.L3_DownRight => L3_DownRightIcon,
            SkillInput.L3_Up => L3_UpIcon,
            SkillInput.L3_Down => L3_DownIcon,

            SkillInput.Hold_Button_X => ButtonXHoldIcon,
            SkillInput.Hold_Button_Triangle => ButtonTriangleHoldIcon,
            SkillInput.Hold_Button_Circle => ButtonCircleHoldIcon,
            SkillInput.Hold_Button_Square => ButtonSquareHoldIcon,


            // R3 Direction Icons
            SkillInput.R3_RightToUp => R3_RightToUpIcon,
            SkillInput.R3_RightToDown => R3_RightToDownIcon,
            SkillInput.R3_UpToRight => R3_UpToRightIcon,
            SkillInput.R3_UpToLeft => R3_UpToLeftIcon,
            SkillInput.R3_DownToRight => R3_DownToRightIcon,
            SkillInput.R3_DownToLeft => R3_DownToLeftIcon,
            SkillInput.R3_LeftToUp => R3_LeftToUpIcon,
            SkillInput.R3_LeftToDown => R3_LeftToDownIcon,
            SkillInput.R3_LeftToDownToLeft => R3_LeftToDownToLeftIcon,
            SkillInput.R3_LeftToUpToLeft => R3_LeftToUpToLeftIcon,

            SkillInput.L3_RightToUp => L3_RightToUpIcon,
            SkillInput.L3_RightToDown => L3_RightToDownIcon,
            SkillInput.L3_UpToRight => L3_UpToRightIcon,
            SkillInput.L3_UpToLeft => L3_UpToLeftIcon,
            SkillInput.L3_DownToRight => L3_DownToRightIcon,
            SkillInput.L3_DownToLeft => L3_DownToLeftIcon,
            SkillInput.L3_LeftToUp => L3_LeftToUpIcon,
            SkillInput.L3_LeftToDown => L3_LeftToDownIcon,
            SkillInput.L3_LeftToDownToLeft => L3_LeftToDownToLeftIcon,
            SkillInput.L3_LeftToUpToLeft => L3_LeftToUpToLeftIcon,
            SkillInput.L3_DownToRightToDown => L3_DownToRightToDown,
            SkillInput.L3_RightToDownToRight => L3_RightToDownToRight,

            // Hold Right Stick Directions
            SkillInput.Hold_R3_Left => Hold_R3_LeftIcon,
            SkillInput.Hold_R3_UpLeft => Hold_R3_UpLeftIcon,
            SkillInput.Hold_R3_DownLeft => Hold_R3_DownLeftIcon,
            SkillInput.Hold_R3_Right => Hold_R3_RightIcon,
            SkillInput.Hold_R3_UpRight => Hold_R3_UpRightIcon,
            SkillInput.Hold_R3_DownRight => Hold_R3_DownRightIcon,
            SkillInput.Hold_R3_Up => Hold_R3_UpIcon,
            SkillInput.Hold_R3_Down => Hold_R3_DownIcon,

            // Hold L3 Directions
            SkillInput.Hold_L3_Left => Hold_L3_LeftIcon,
            SkillInput.Hold_L3_UpLeft => Hold_L3_UpLeftIcon,
            SkillInput.Hold_L3_DownLeft => Hold_L3_DownLeftIcon,
            SkillInput.Hold_L3_Right => Hold_L3_RightIcon,
            SkillInput.Hold_L3_UpRight => Hold_L3_UpRightIcon,
            SkillInput.Hold_L3_DownRight => Hold_L3_DownRightIcon,
            SkillInput.Hold_L3_Up => Hold_L3_UpIcon,
            SkillInput.Hold_L3_Down => Hold_L3_DownIcon,

            SkillInput.Hold_L3_Any => Hold_L3_AnyIcon,
            SkillInput.Hold_R3_Any => Hold_R3_AnyIcon,

            SkillInput.L2_None => L2Icon,
            SkillInput.R2_None => R2Icon,

            SkillInput.None => string.Empty,
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Creates a formatted display string representing a sequence of possible skill inputs.
    /// Supports highlighting a specific index in the sequence.
    /// </summary>
    /// <param name="possibleSequences">List of possible input sequences.</param>
    /// <param name="highlightIndex">Index of input to highlight (default -1 for no highlight).</param>
    /// <param name="highlight">Whether to apply highlight color.</param>
    /// <returns>Formatted string showing inputs at each sequence step with OR between alternatives.</returns>
    public string GetSequenceDisplay(List<SkillInputHolder> possibleSequences, int highlightIndex = -1, bool highlight = false)
    {
        string display = "";

        int sequenceLength = 0;
        if (possibleSequences.Count > 0) { sequenceLength = possibleSequences[0].input.Count; }

        for (int i = 0; i < sequenceLength; i++)
        {
            List<string> possibleInputsAtPosition = new List<string>();

            for (int seqIndex = 0; seqIndex < possibleSequences.Count; seqIndex++)
            {
                string inputStr = GetIconForSkillInput(possibleSequences[seqIndex].input[i]);

                if (highlight && i == highlightIndex)
                {
                    inputStr = AddSpriteColor(inputStr, "#55FF55FF");
                }

                if (!possibleInputsAtPosition.Contains(inputStr))
                {
                    possibleInputsAtPosition.Add(inputStr);
                }
            }

            if (possibleInputsAtPosition.Count > 1)
            {
                display += string.Join(" OR ", possibleInputsAtPosition);
            }
            else
            {
                display += possibleInputsAtPosition[0];
            }

            if (i < sequenceLength - 1)
            {
                display += " - ";
            }
        }

        return display;
    }

    private string AddSpriteColor(string spriteTag, string colorHex)
    {
        if (spriteTag.Contains("color=")) return spriteTag;

        int insertPos = spriteTag.IndexOf('>');
        if (insertPos > 0)
        {
            return spriteTag.Insert(insertPos, $" color={colorHex}");
        }

        return spriteTag;
    }
}