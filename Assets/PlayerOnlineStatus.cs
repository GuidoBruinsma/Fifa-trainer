using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerOnlineStatus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI onlineStatus;
    [SerializeField] private TextMeshProUGUI playerName;

    void Update()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            onlineStatus.text = "Status: <color=green>Online</color>";
            playerName.text = "Player name: " + AuthenticationService.Instance.PlayerName;
        }
        else
        {
            onlineStatus.text = "Status: <color=red>Offline</color>";
            playerName.text = "Player name: " + AuthenticationService.Instance.PlayerName;
        }
    }
}
