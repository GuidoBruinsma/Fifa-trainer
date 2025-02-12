using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private Dictionary<float, int> scoreData = new();
    public int score;
    private float timeElapsed;
    private void Start()
    {
        EventManager.OnSequenceSuccess.AddListener(ScoreIncrease);
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;
        UI_Manager.Instance?.SetElapsedTimeText(timeElapsed);
    }

    private void ScoreIncrease()
    {
        score++;
        //Debug.Log("poepeope");
    }
}
