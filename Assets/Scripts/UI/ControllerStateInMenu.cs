using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllerStateInMenu : MonoBehaviour
{
    [SerializeField] private EventSystem system;
    [SerializeField] private TextMeshProUGUI controllerStateText;

    private void Update()
    {
        Gamepad pad = Gamepad.current;
        if (pad == null)
        {
            controllerStateText.text = "<color=red>Controller not connected</color>";

            var b = transform.GetComponentsInChildren<Button>();
            foreach (var item in b)
            {
                item.interactable = false;
            }
        }
        else
        {
            controllerStateText.text = "<color=green>Controller connected</color>";
            var b = transform.GetComponentsInChildren<Button>();
            foreach (var item in b)
            {
                item.interactable = true;
            }
            if (system.currentSelectedGameObject == null)
            {
                system.SetSelectedGameObject(transform.GetComponentInChildren<Button>().gameObject);
            }
        }
    }
}
