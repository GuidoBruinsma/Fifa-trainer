using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerHandler : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;

    void OnEnable() => EventManager.OnSkillChanged.AddListener(PlayCurrentSkillTutorial);


    private void PlayCurrentSkillTutorial(Skill skill)
    {
        Debug.Log("new skill " + skill.moveName);
        player.clip = skill.clip;
    }
}
