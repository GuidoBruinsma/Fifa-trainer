using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates a UI slider and text to display the success rate of a given skill.
/// Includes smooth animation of slider value changes and a pop effect for feedback.
/// </summary>
public class SuccessRate_Slider : MonoBehaviour
{
    [SerializeField] private Skill currentSkill;

    [Header("UI References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Visual Feedback")]
    [SerializeField] private Gradient successGradient;
    [SerializeField] private float lerpSpeed = 5f;

    private float targetValue;

    /// <summary>
    /// Called before Start. Ensures slider reference is assigned.
    /// Subscribes to the global skill change event.
    /// </summary>
    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        EventManager.OnSkillChanged.AddListener(SkillChanged);
    }

    /// <summary>
    /// Updates the percentage text to reflect the current slider value every frame.
    /// </summary>
    private void Update()
    {

        if (percentageText != null)
        {
            percentageText.text = Mathf.RoundToInt(slider.value * 100f) + "%";
        }
    }

    /// <summary>
    /// Callback for when the skill changes.
    /// Updates the target slider value and starts the pop animation coroutine.
    /// </summary>
    /// <param name="newSkill">The newly selected skill</param>
    private void SkillChanged(Skill newSkill)
    {
        if (newSkill == currentSkill)
        {

            targetValue = currentSkill.SuccessRate;
            StartCoroutine(AnimatePop());
        }
        else
        {

            currentSkill = newSkill;
            targetValue = currentSkill.SuccessRate;

            slider.value = 0;

            StartCoroutine(AnimatePop());
        }
    }

    /// <summary>
    /// Coroutine that smoothly animates the slider value to the target value.
    /// Uses Lerp with deltaTime for smooth interpolation.
    /// </summary>
    private IEnumerator AnimatePop()
    {
        if (slider.value < targetValue)
        {
            while (slider.value < targetValue)
            {
                slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * lerpSpeed);
                yield return null;
            }
        }
        else {
            while (slider.value > targetValue)
            {
                slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * lerpSpeed);
                yield return null;
            }
        }
    }
}
