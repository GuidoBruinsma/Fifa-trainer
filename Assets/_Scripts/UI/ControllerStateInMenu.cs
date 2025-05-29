using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Manages the controller connection state display in the menu
/// and enables/disables UI buttons accordingly.
/// </summary>
public class ControllerStateInMenu : MonoBehaviour
{
    [SerializeField] private EventSystem system;
    [SerializeField] private TextMeshProUGUI controllerStateText;
    
    private Button[] buttons;

    /// <summary>
    /// Initializes the buttons array and updates controller state display on start.
    /// </summary>
    private void Start()
    {
        buttons = GetComponentsInChildren<Button>(true); // include inactive
        UpdateControllerState();
    }

    /// <summary>
    /// Updates the controller state every frame.
    /// </summary>
    private void Update()
    {
        UpdateControllerState();
    }

    /// <summary>
    /// Checks if a gamepad controller is connected, updates the UI text color and message,
    /// enables/disables buttons, and sets the selected UI button if none is selected.
    /// </summary>
    private void UpdateControllerState()
    {
        Gamepad pad = Gamepad.current;

        if (pad == null)
        {
            controllerStateText.text = "<color=red>Controller not connected</color>";
            foreach (var btn in buttons)
                btn.interactable = false;
        }
        else
        {
            controllerStateText.text = "<color=green>Controller connected</color>";
            foreach (var btn in buttons)
                btn.interactable = true;

            if (system.currentSelectedGameObject == null || !system.currentSelectedGameObject.activeInHierarchy)
            {
                foreach (var btn in buttons)
                {
                    if (btn.gameObject.activeInHierarchy)
                    {
                        system.SetSelectedGameObject(btn.gameObject);
                        break;
                    }
                }
            }
        }
    }
}
