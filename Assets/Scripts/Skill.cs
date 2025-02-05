using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillMove", menuName = "Skill Moves/Skill")]
public class Skill : ScriptableObject
{
    public string moveName;
    public List<SkillInput> inputSequence;
    public float maxTimeBetweenInput;
}

public enum SkillInput { 
    L1, L2, L3, R1, R2, R3,
    Button_X, Button_Triangle, Button_Circle, Button_Square,
    L3_Rotate, R3_Rotate
}
