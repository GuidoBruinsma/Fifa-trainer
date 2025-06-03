using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

/// <summary>
/// Displays the player's online status and player name using Unity Services Authentication.
/// Updates UI text each frame to reflect the current authentication state.
/// </summary>
public class PlayerOnlineStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI onlineStatus;
    [SerializeField] private TextMeshProUGUI playerName;

    /// <summary>
    /// Called every frame to update the displayed player status and name.
    /// </summary>
    void Update()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            onlineStatus.text = "Status: <color=green>Online</color>";
            playerName.text = $"Player name: <color=green>{AuthenticationService.Instance.PlayerName}</color>";
        }
        else
        {
            onlineStatus.text = "Status: <color=red>Offline</color>";
            playerName.text = $"Player name: <color=green>{AuthenticationService.Instance.PlayerName}</color>";
        }
    }
}
