using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequenceVisualizer : MonoBehaviour
{
    [SerializeField] private string buttonXTapIcon;
    [SerializeField] private string buttonCircleTapIcon;
    [SerializeField] private string buttonSquareTapIcon;
    [SerializeField] private string buttonTriangleTapIcon;

    [SerializeField] private string L2HoldIcon;
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI sequenceText;

    public string RS_UpLeftIcon;

    public void VisualizeSequence(List<SkillInput> sequence, int currentIndex)
    {
        string display = "";

        for (int i = 0; i < sequence.Count; i++)
        {
            string inputStr = SkillInputToString(sequence[i]);

            if (i == currentIndex)
            {
                inputStr = $"<color=green>{inputStr}</color>";
            }

            display += inputStr;

            if (i < sequence.Count - 1)
            {
                display += " - ";
            }
        }

        sequenceText.text = display;
    }
    public string SkillInputToString(SkillInput input)
    {
        return input switch
        {
            SkillInput.Button_X => buttonXTapIcon,
            SkillInput.Button_Circle => buttonCircleTapIcon,
            SkillInput.Button_Square => buttonSquareTapIcon,
            SkillInput.Button_Triangle => buttonTriangleTapIcon,
            SkillInput.L2_Hold => L2HoldIcon,
            SkillInput.RS_UpLeft => RS_UpLeftIcon,
            _ => null,
        };
    }
}
