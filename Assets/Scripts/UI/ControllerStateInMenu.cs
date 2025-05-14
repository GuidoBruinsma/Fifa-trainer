using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerStateInMenu : MonoBehaviour
{
    [SerializeField] private EventSystem system;
    [SerializeField] private TextMeshProUGUI controllerStateText;
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>(true); // include inactive
        UpdateControllerState();
    }

    private void Update()
    {
        UpdateControllerState();
    }

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

            if (system.currentSelectedGameObject == null)
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
