using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages the UI selector behavior, such as tracking button selection and updating visual effects.
/// </summary>
public class SelectorManager : MonoBehaviour
{
    [Header("Shader Settings")]
    private const string shaderProperty = "_RunEffect";
    [SerializeField] private Material material;

    [Header("UI References")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Button[] buttons;

    [Header("Lerp Settings")]
    [SerializeField] private float interpolationSpeed = 10f;
    [SerializeField] private float threshold = 0.9f;

    [SerializeField] private Vector2 adaptSize;
    private RectTransform selectorRect;
    private bool hasTriggered = false;

    /// <summary>
    /// Called when the GameObject is enabled. Initializes selector and sets up button triggers.
    /// </summary>
    private void OnEnable()
    {
        selectorRect = GetComponent<RectTransform>();

        if (buttons == null || buttons.Length == 0)
        {
            buttons = transform.parent.GetComponentsInChildren<Button>(true);
        }

        foreach (var b in buttons)
        {
            AddEventTriggerComponent(b);
        }
    }

    /// <summary>
    /// Adds a pointer enter trigger to a button to update selection.
    /// </summary>
    /// <param name="b">The button to add the event trigger to.</param>
    private void AddEventTriggerComponent(Button b)
    {
        EventTrigger eventTrigger = b.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = b.gameObject.AddComponent<EventTrigger>();

        eventTrigger.triggers.Clear();

        EventTrigger.Entry entry = new();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) =>
        {
            eventSystem.SetSelectedGameObject(b.gameObject);
        });

        eventTrigger.triggers.Add(entry);
    }

    /// <summary>
    /// Updates the selector position and size each frame to follow the selected button.
    /// </summary>
    private void Update()
    {
        float lerpSpeed = Time.deltaTime * interpolationSpeed;
        SetSelectorToButtonPosition(lerpSpeed);

        var currentButtons = transform.parent.GetComponentsInChildren<Button>(false);
        if (currentButtons.Length != buttons.Length)
        {
            buttons = currentButtons;

            if (buttons.Length > 0)
            {
                eventSystem.SetSelectedGameObject(buttons[0].gameObject);

                foreach (var b in buttons)
                {
                    AddEventTriggerComponent(b);
                }
            }
        }
    }


    /// <summary>
    /// Smoothly moves and resizes the selector to match the currently selected button.
    /// </summary>
    /// <param name="lerpSpeed">The speed at which to interpolate position and size.</param>
    private void SetSelectorToButtonPosition(float lerpSpeed)
    {
        GameObject selected = eventSystem.currentSelectedGameObject;

        if (selected == null || !selected.TryGetComponent<RectTransform>(out var selectedRect))
            return;
        selectorRect.position = Vector3.Lerp(selectorRect.position, selectedRect.position, lerpSpeed);
        selectorRect.sizeDelta = Vector2.Lerp(selectorRect.sizeDelta, selectedRect.sizeDelta + adaptSize, lerpSpeed);
        selectorRect.pivot = selectedRect.pivot;

        bool isAtTarget = Vector3.Distance(selectorRect.position, selectedRect.position) < threshold;

        if (!isAtTarget)
        {
            if (!hasTriggered)
            {
                SetShaderEffect(true);
                hasTriggered = true;
            }
        }
        else
        {
            if (hasTriggered)
            {
                SetShaderEffect(false);
                hasTriggered = false;
            }
        }
    }

    /// <summary>
    /// Toggles the selector material's shader effect on or off.
    /// </summary>
    /// <param name="enabled">Whether to enable or disable the shader effect.</param>
    private void SetShaderEffect(bool enabled)
    {
        if (material != null && material.HasProperty(shaderProperty))
        {
            material.SetFloat(shaderProperty, enabled ? 1f : 0f);
        }
    }
}
