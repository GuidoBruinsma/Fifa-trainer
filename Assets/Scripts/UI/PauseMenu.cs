using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;

    [SerializeField] private GameObject pauseCanvas;

    /// <summary>
    /// Add buttons in array as they are in the scene. First Resume, Settings, Quit, Etc.
    /// </summary>
    [SerializeField] private GameObject[] buttons;

    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;

    private InputAction _Buttons;

    private bool isPaused = false;

    private void Awake()
    {
        pauseCanvas.SetActive(isPaused);

        InputActionMap map = controls.FindActionMap("DualShock");

        _Buttons = map.FindAction("Buttons");
        _Buttons.performed += ctx => { ActivatePause(ctx.control.name); };
    }

    private void Update()
    {

    }

    //TODO: Uncomment, delete saving
    private void ActivatePause(string buttonName)
    {

        if (buttonName == "select") isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        pauseCanvas.SetActive(isPaused);
    }

    public void ResumePause() {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        pauseCanvas.SetActive(isPaused);
    }
}
