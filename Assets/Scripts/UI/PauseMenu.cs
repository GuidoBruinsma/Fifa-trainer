using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;

    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;

    private InputAction _Buttons;

    [SerializeField] private bool isPaused = false;

    private void Start()
    {
        pauseCanvas.SetActive(isPaused);

        InputActionMap map = controls.FindActionMap("DualShock");

        _Buttons = map.FindAction("Buttons");
        _Buttons.performed += ctx => { ActivatePause(ctx.control.name); };
    }

    //TODO: Uncomment, delete saving
    private void ActivatePause(string buttonName)
    {
        Debug.Log(buttonName);
        if (buttonName == "start") isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        pauseCanvas.SetActive(isPaused);
    }

    public void ResumePause() {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        pauseCanvas.SetActive(isPaused);
    }
}
