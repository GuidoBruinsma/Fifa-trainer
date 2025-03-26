using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SkillGameSettings", menuName = "Game Settings/SkillGameSettings")]
public class SkillGameSettings : ScriptableObject
{
    public List<Skill> allSkillMoves;

    public List<Skill> selectedSkillMoves;

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
