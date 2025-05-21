using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the pause menu UI and game pause state.
/// Listens to controller button input to toggle pause.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;

    [Header("Controls")]
    [SerializeField] private InputActionAsset controls;

    private InputAction _Buttons;

    [SerializeField] private bool isPaused = false;
   
    /// <summary>
    /// Initializes the pause menu by setting the pause canvas active state
    /// and subscribing to the button input action to listen for pause toggling.
    /// </summary>
    private void Start()
    {
        pauseCanvas.SetActive(isPaused);

        InputActionMap map = controls.FindActionMap("DualShock");

        _Buttons = map.FindAction("Buttons");
        _Buttons.performed += ctx => { ActivatePause(ctx.control.name); };
    }

    /// <summary>
    /// Toggles pause state when the designated button (e.g., "start") is pressed.
    /// Pauses or resumes the game by changing Time.timeScale,
    /// and activates or deactivates the pause canvas UI.
    /// </summary>
    /// <param name="buttonName">The name of the button pressed</param>
    private void ActivatePause(string buttonName)
    {
        Debug.Log(buttonName);
        if (buttonName == "start") isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        pauseCanvas.SetActive(isPaused);
    }

    /// <summary>
    /// Method to resume the game from pause.
    /// Toggles pause state off, resumes time, and hides pause UI.
    /// Can be hooked to a UI button to resume from pause.
    /// </summary>
    public void ResumePause() {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;

        pauseCanvas.SetActive(isPaused);
    }
}
