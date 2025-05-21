using System.Threading.Tasks;
using TMPro;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

/// <summary>
/// Handles leaderboard interactions such as submitting scores, displaying the leaderboard, and updating entries in real-time.
/// </summary>
public class Leaderboard : MonoBehaviour
{
    [SerializeField] private string leaderboardId;

    [SerializeField] private Transform leaderboardParent;
    [SerializeField] private Transform leaderboardContentParent;
    [SerializeField] private Transform leaderboardItem;
    
    /// <summary>
    /// Submits a random score to the leaderboard for demonstration/testing purposes.
    /// </summary>
    public async void SubmitScore()
    {
        int s = Random.Range(0, 1000);
        Debug.Log($"Scores added: {s}");
        await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, s);
    }

    /// <summary>
    /// Opens the leaderboard UI and begins updating the leaderboard in real-time.
    /// </summary>
    public void OpenLeaderboard()
    {
        leaderboardParent.gameObject.SetActive(true);
        UpdateLeaderboard();
    }

    /// <summary>
    /// Continuously fetches and displays leaderboard scores while the leaderboard UI is active.
    /// </summary>
    public async void UpdateLeaderboard()
    {
        while (Application.isPlaying && leaderboardParent.gameObject.activeInHierarchy)
        {
            LeaderboardScoresPage scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);
            foreach (Transform item in leaderboardContentParent)
            {
                Destroy(item.gameObject);
            }
            foreach (LeaderboardEntry s in scores.Results)
            {
                Transform go = Instantiate(leaderboardItem);
                go.SetParent(leaderboardContentParent, false);

                string input = s.PlayerName;

                int index = input.IndexOf('#');
                if (index != -1)
                {
                    string result = input.Substring(0, index);
                    go.GetChild(0).GetComponent<TextMeshProUGUI>().text = result;
                }
                else {
                    go.GetChild(0).GetComponent<TextMeshProUGUI>().text = input;
                }

                go.GetChild(1).GetComponent<TextMeshProUGUI>().text = s.Score.ToString();
            }
            await Task.Delay(500);
        }
    }
}
