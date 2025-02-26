using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMove", menuName = "Skill Moves/Skill")]
public class Skill : ScriptableObject
{
    [TextArea(3, 5)]
    public string importantNode;

    [Space]
    public string moveName;
    public List<SkillInputHolder> inputSequence;


    public float maxTimeBetweenInput;

    [Space]
    [Header("Add only if the skill contains R Rotate or L Rotate")]
    public SkillInput[] rotationSequence;

    [Space]
    [Header("Add only if the skill has variations (Hold X or Hold O). \n " +
        "Use this as the X or O and the \n " +
        "other sequence as the other sequence")]
    public List<SkillInput> secondInputSequence;
    public SkillInput[] secondRotationSequence;
}

[Serializable]
public class SkillInputHolder
{
    public List<SkillInput> input;
}

public enum SkillInput
{
    L1, L2, L3, L1_Hold, L2_Hold, L3_Rotate, L3_Any, Hold_L3_Any,

    [Space]
    R1, R2, R3, R1_Hold, R2_Hold, R3_Rotate, Hold_R3_Any,

    [Space]
    Button_X, Button_Triangle, Button_Circle, Button_Square,

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
    

    [Space]
    R3_RightToUp, R3_RightToDown, 

    R3_UpToRight, R3_UpToLeft,

    R3_DownToRight, R3_DownToLeft,

    R3_LeftToUp, R3_LeftToDown, R3_LeftToDownToLeft, R3_LeftToUpToLeft,


    [Space]
    Hold_R3_Left, Hold_R3_UpLeft, Hold_R3_DownLeft,
    Hold_R3_Right, Hold_R3_UpRight, Hold_R3_DownRight,
    Hold_R3_Up, Hold_R3_Down, Hold_R3_None,

    [Space]
    Hold_L3_Left, Hold_L3_UpLeft, Hold_L3_DownLeft,
    Hold_L3_Right, Hold_L3_UpRight, Hold_L3_DownRight,
    Hold_L3_Up, Hold_L3_Down, Hold_L3_None,

    [Space]
    L2_None, R2_None
}
