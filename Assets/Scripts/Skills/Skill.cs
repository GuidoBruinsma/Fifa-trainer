using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMove", menuName = "Skill Moves/Skill")]
public class Skill : ScriptableObject
{
    public string moveName;
    public List<SkillInput> inputSequence;
    public float maxTimeBetweenInput;

    [Space]
    [Header("Add only if the skill contains R Rotate or L Rotate")]
    public SkillInput[] rotationSequence;
}

public enum SkillInput
{
    L1, L2, L3, R1, R2, R3,

    Button_X, Button_Triangle, Button_Circle, Button_Square,
    
    L3_Rotate, R3_Rotate,

    L1_Hold, L2_Hold, 
    R1_Hold, R2_Hold,


    RS_Left, RS_UpLeft, RS_DownLeft,
    RS_Right, RS_UpRight, RS_DownRight,
    RS_Up, RS_Down, RS_None,

    L2_None, R2_None    
}
