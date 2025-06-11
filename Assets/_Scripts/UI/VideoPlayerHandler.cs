using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerHandler : MonoBehaviour
{
    [SerializeField] private VideoPlayer player;

    void OnEnable() => EventManager.OnSkillChanged.AddListener(PlayCurrentSkillTutorial);


    private void PlayCurrentSkillTutorial(Skill skill)
    {
        player.clip = skill.clip;
    }
}
