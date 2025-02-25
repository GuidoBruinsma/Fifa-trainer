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
    R1, R2, R3, R1_Hold, R2_Hold, R3_Rotate, R3_Any,

    [Space]
    Button_X, Button_Triangle, Button_Circle, Button_Square,

    [Space]
    Hold_Button_X, Hold_Button_Triangle, Hold_Button_Circle, Hold_Button_Square,

    [Space]
    RS_Left, RS_UpLeft, RS_DownLeft,
    RS_Right, RS_UpRight, RS_DownRight,
    RS_Up, RS_Down, RS_None,

    [Space]
    LS_Left, LS_UpLeft, LS_DownLeft,
    LS_Right, LS_UpRight, LS_DownRight,
    LS_Up, LS_Down, LS_None,

    [Space]
    Hold_RS_Left, Hold_RS_UpLeft, Hold_RS_DownLeft,
    Hold_RS_Right, Hold_RS_UpRight, Hold_RS_DownRight,
    Hold_RS_Up, Hold_RS_Down, Hold_RS_None,

    [Space]
    Hold_LS_Left, Hold_LS_UpLeft, Hold_LS_DownLeft,
    Hold_LS_Right, Hold_LS_UpRight, Hold_LS_DownRight,
    Hold_LS_Up, Hold_LS_Down, Hold_LS_None,

    [Space]
    L2_None, R2_None
}
