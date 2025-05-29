using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a skill move using an input sequence and various metadata.
/// </summary>
[CreateAssetMenu(fileName = "SkillMove", menuName = "Skill Moves/Skill")]
public class Skill : ScriptableObject
{
    [TextArea(3, 5)]
    public string importantNode;

    [Space]
    public string moveName;
    public List<SkillInputHolder> inputSequence;

    public float maxTimeBetweenInput;
    public float difficultyScore;

    public int attempts;
    public int successes;

    /// <summary>
    /// The success rate for this skill (0 to 1).
    /// </summary>
    public float SuccessRate => attempts == 0 ? 0f : (float)successes / attempts;

    /// <summary>
    /// Resets the attempt and success stats.
    /// </summary>
    public void ResetStats() { 
        attempts = 0; 
        //successes = 0;
    }

    /// <summary>
    /// Sends analytics data for this skill (data structure only—no external call).
    /// </summary>
    public void SendAnalytics() {

        SkillChartData skillAnalyzeData = new SkillChartData
        { 
            skillName = moveName,
            attempts = attempts,
            successes = successes,
            successRate = SuccessRate
        };
    }
}

/// <summary>
/// Represents one step in a skill sequence, which can contain multiple simultaneous inputs.
/// </summary>
[Serializable]
public class SkillInputHolder
{
    public List<SkillInput> input;
}

/// <summary>
/// Enumeration of all possible skill inputs.
/// </summary>
public enum SkillInput
{
    //L Buttons
    L1, L2, L3, L1_Hold, L2_Hold, L3_Rotate, L3_Any, Hold_L3_Any,

    //R Buttons
    [Space]
    R1, R2, R3, R1_Hold, R2_Hold, R3_Rotate, R3_Any, Hold_R3_Any,

    //General Button Tab
    [Space]
    Button_X, Button_Triangle, Button_Circle, Button_Square,

    //General Button Hold
    [Space]
    Hold_Button_X, Hold_Button_Triangle, Hold_Button_Circle, Hold_Button_Square,

    //Old version////////////////////////////////////////////////////
    [Space]
    R3_Left, R3_UpLeft, R3_DownLeft,
    R3_Right, R3_UpRight, R3_DownRight,
    R3_Up, R3_Down, R3_None,

    [Space]
    L3_Left, L3_UpLeft, L3_DownLeft,
    L3_Right, L3_UpRight, L3_DownRight,
    L3_Up, L3_Down, L3_None,
    ///////////////////////////////////////////////////////////////

    //Rotation buttons R3
    [Space]
    R3_RightToUp, R3_RightToDown,

    R3_UpToRight, R3_UpToLeft,

    R3_DownToRight, R3_DownToLeft,

    R3_LeftToUp, R3_LeftToDown, R3_LeftToDownToLeft, R3_LeftToUpToLeft,

    //Rotation buttons L3
    [Space]
    L3_RightToUp, L3_RightToDown,

    L3_UpToRight, L3_UpToLeft,

    L3_DownToRight, L3_DownToLeft,

    L3_LeftToUp, L3_LeftToDown, L3_LeftToDownToLeft, L3_LeftToUpToLeft,

    L3_DownToRightToDown, L3_RightToDownToRight,

    //Hold Direction R3
    [Space]
    Hold_R3_Left, Hold_R3_UpLeft, Hold_R3_DownLeft,
    Hold_R3_Right, Hold_R3_UpRight, Hold_R3_DownRight,
    Hold_R3_Up, Hold_R3_Down, Hold_R3_None,

    //Hold Direction L3
    [Space]
    Hold_L3_Left, Hold_L3_UpLeft, Hold_L3_DownLeft,
    Hold_L3_Right, Hold_L3_UpRight, Hold_L3_DownRight,
    Hold_L3_Up, Hold_L3_Down, Hold_L3_None,

    //None
    [Space]
    L2_None, R2_None, Hold_None, Flick_None, None
}
