using System.Collections.Generic;
using UnityEngine;

public class SequenceVisualizer : MonoBehaviour
{
    [SerializeField] private SkillControlIconMap skillMap;

    public void VisualizeSequence(List<SkillInputHolder> sequence, int currentIndex)
    {
        string display = skillMap.GetSequenceDisplay(sequence, currentIndex, true);
        UI_Manager.Instance?.SetSkillMoveInfo(display);
    }
}
