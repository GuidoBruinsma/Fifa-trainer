using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for visualizing a skill move sequence on the UI.
/// </summary>
public class SequenceVisualizer : MonoBehaviour
{
    [SerializeField] private SkillControlIconMap skillMap;

    /// <summary>
    /// Visualizes the given sequence of skill inputs on the UI.
    /// </summary>
    /// <param name="sequence">The list of skill inputs to display.</param>
    /// <param name="currentIndex">The current index in the sequence (highlighted).</param>
    public void VisualizeSequence(List<SkillInputHolder> sequence, int currentIndex)
    {
        string display = skillMap.GetSequenceDisplay(sequence, currentIndex, true);
        UI_Manager.Instance?.SetSkillMoveInfo(display);
    }
}
