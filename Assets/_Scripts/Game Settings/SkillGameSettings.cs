using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds and manages the settings for the skill game, including all available and selected skill moves.
/// </summary>
[CreateAssetMenu(fileName = "SkillGameSettings", menuName = "Game Settings/SkillGameSettings")]
public class SkillGameSettings : ScriptableObject
{
    public List<Skill> allSkillMoves;

    public List<Skill> selectedSkillMoves;

    /// <summary>
    /// Sorts and selects skill moves based on the given difficulty score, then loads the main skill scene.
    /// </summary>
    /// <param name="difScore">The difficulty score to filter skill moves by.</param>
    public void SortMoves(float difScore)
    {
        selectedSkillMoves.Clear();

        foreach (var skill in allSkillMoves)
        {
            if (difScore == skill.difficultyScore)
                selectedSkillMoves.Add(skill);
        }

        SceneManager.LoadScene(1);
    }
}
