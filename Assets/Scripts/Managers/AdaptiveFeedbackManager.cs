using TMPro;
using UnityEngine;

public class AdaptiveFeedbackManager : MonoBehaviour
{
    /*
     * Pseudo
     * 
     * keep track on how many moves has been performed
     * if divide the total moves with success moves will get the percentage of success
     * if is > 80% that means is going good and difficulty needs to increase
     * if i'ts less, = bad performance = easier
     * 
     */
    [SerializeField] private TextMeshProUGUI difficultyScoreText;

    [SerializeField] private float difficultyScore;

    [SerializeField] private int total;
    [SerializeField] private int successful;

    [SerializeField] private int adjustmentRate;

    private void Start()
    {
        EventManager.OnSkillIsCompleted.AddListener(ReceivedScore);
    }

    void ReceivedScore(bool isCompleted)
    {
        total++;

        if (isCompleted)
            successful++;

        if (total % adjustmentRate == 0)
        {
            AdjustDifficulty();
        }
    }

    // TODO: 
    void AdjustDifficulty()
    {
        float successRate = (float)successful / total;

        difficultyScoreText.text = "Difficulty: " + successRate.ToString();

        if (successRate > 0.8f)
        { 
            //Make it harder if the success rate is > 80%
            //more difficult moves?
            //more time to execute?
        }
        if (successRate < 0.5f)
        {
            //make it easier
        }
    }
}
