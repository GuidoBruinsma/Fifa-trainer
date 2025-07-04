using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages UI elements related to skill moves, timers, and move sequences.
/// Implements a singleton pattern for global access.
/// </summary>
public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI sequenceName;
    [SerializeField] private TextMeshProUGUI sequenceText;
    
    /// <summary>
    /// Initializes the singleton instance or destroys duplicate instances.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    
    /// <summary>
    /// Sets the current skill move name and its input sequence on the UI.
    /// </summary>
    /// <param name="sequence">The move input sequence as string.</param>
    public void SetSkillMoveInfo(string sequence)
    {
        //sequenceName.text = SkillMovesManager.CurrentSkill.moveName;
        sequenceText.text = sequence;
    }

    //Delete This
    public void SetName(string name) {
        sequenceName.text = name;
    }
}
