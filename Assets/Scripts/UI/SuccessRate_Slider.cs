using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float popDuration = 0.1f;

    private float targetValue;
    private Vector3 originalScale;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        EventManager.OnSkillChanged.AddListener(SkillChanged);
    }

    private void Update()
    {

        if (percentageText != null)
        {
            percentageText.text = Mathf.RoundToInt(slider.value * 100f) + "%";
        }
    }

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
