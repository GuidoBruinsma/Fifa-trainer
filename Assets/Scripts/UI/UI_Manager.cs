using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timeleftText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetTimerText(float timeLeft) {
        timeleftText.text = timeLeft.ToString("0.0");
    }
}
