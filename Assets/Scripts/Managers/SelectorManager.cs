using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private RectTransform selectorRect;
    private bool hasTriggered = false;
    public Vector2 adaptSize;
    public GameObject go;

    private void OnEnable()
    {
        selectorRect = GetComponent<RectTransform>();

        if (buttons == null || buttons.Length == 0)
        {
            buttons = transform.parent.GetComponentsInChildren<Button>(true);
        }
    }

    private void Update()
    {
        go = eventSystem.currentSelectedGameObject;
        float lerpSpeed = Time.deltaTime * interpolationSpeed;
        SetSelectorToButtonPosition(lerpSpeed);
    }

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

    private void SetShaderEffect(bool enabled)
    {
        if (material != null && material.HasProperty(shaderProperty))
        {
            material.SetFloat(shaderProperty, enabled ? 1f : 0f);
        }
    }
}
