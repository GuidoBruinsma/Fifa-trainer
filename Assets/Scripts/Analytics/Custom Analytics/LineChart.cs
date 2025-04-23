using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChart : MonoBehaviour
{
    public enum SkillDataType
    {
        Attempts,
        Successes,
        SuccessRate
    };

    public SkillDataType _skillDataType;

    public RectTransform chartContainer;
    public Sprite circleSprite;

    private List<GameObject> chartObjects = new();
    private List<RectTransform> pointRects = new();
    private List<SkillAnalyzeData> skillAnalyzeHistory = new();

    private void Start() => EventManager.OnAnalyzeSkillDataSent.AddListener(ReceiveData);
    private void OnDisable() => EventManager.OnAnalyzeSkillDataSent.RemoveListener(ReceiveData);

    //TODO: Add X axis
    private void ReceiveData(SkillAnalyzeData skillData)
    {
        skillAnalyzeHistory.Add(skillData);

        // Optional: limit to N data points (e.g., last 20)
        if (skillAnalyzeHistory.Count > 10)
            skillAnalyzeHistory.RemoveAt(0);

        List<float> values = new();

        switch (_skillDataType)
        {
            case SkillDataType.Attempts:
                foreach (var s in skillAnalyzeHistory)
                    values.Add(s.attempts);
                break;

            case SkillDataType.Successes:
                foreach (var s in skillAnalyzeHistory)
                    values.Add(s.successes);
                break;

            case SkillDataType.SuccessRate:
                foreach (var s in skillAnalyzeHistory)
                    values.Add(s.successRate * 100f);
                break;
        }

        ShowGraph(values);
    }

    public void ShowGraph(List<float> values)
    {
        ClearDynamicChart();

        float chartHeight = chartContainer.sizeDelta.y;
        float chartWidth = chartContainer.sizeDelta.x;

        float maxYValue = Mathf.Max(1f, Mathf.Max(values.ToArray())); // Prevent divide-by-zero
        float yMaxRounded = Mathf.Ceil(maxYValue / 50f) * 50f; // Round up to nearest 50

        float xSpacing = chartWidth / Mathf.Max(1, (values.Count - 1));

        DrawHorizontalLines(yMaxRounded);

        for (int i = 0; i < values.Count; i++)
        {
            float x = i * xSpacing;
            float y = (values[i] / yMaxRounded) * chartHeight;
            Vector2 currentPos = new Vector2(x, y);

            var point = CreatePoint(currentPos);
            chartObjects.Add(point);
            pointRects.Add(point.GetComponent<RectTransform>());

            var vertical = CreateVerticalLine(x, chartHeight);
            chartObjects.Add(vertical);

            if (i > 0)
            {
                Vector2 prevPos = pointRects[i - 1].anchoredPosition;
                var line = CreateLine(prevPos, currentPos);
                chartObjects.Add(line);
            }
        }
    }

    private void DrawHorizontalLines(float maxY)
    {
        int horizontalSteps = 5;
        float chartHeight = chartContainer.sizeDelta.y;
        float chartWidth = chartContainer.sizeDelta.x;

        for (int i = 0; i <= horizontalSteps; i++)
        {
            float normalized = i / (float)horizontalSteps;
            float yPos = normalized * chartHeight;

            GameObject obj = new GameObject("HorizontalLine", typeof(Image));
            obj.transform.SetParent(chartContainer, false);
            Image img = obj.GetComponent<Image>();
            img.color = Color.white;

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.zero;
            rt.sizeDelta = new Vector2(chartWidth, 2);
            rt.anchoredPosition = new Vector2(chartWidth / 2, yPos);

            chartObjects.Add(obj);

            GameObject label = new GameObject("YLabel", typeof(Text));
            label.transform.SetParent(chartContainer, false);
            Text text = label.GetComponent<Text>();
            text.text = (normalized * maxY).ToString("F0");
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.color = Color.white;

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.zero;
            labelRT.anchoredPosition = new Vector2(-35, yPos);
            labelRT.sizeDelta = new Vector2(60, 20);

            chartObjects.Add(label);
        }
    }

    private GameObject CreatePoint(Vector2 position)
    {
        GameObject obj = new GameObject("Point", typeof(Image));
        obj.transform.SetParent(chartContainer, false);
        obj.GetComponent<Image>().sprite = circleSprite;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(15, 15);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        return obj;
    }

    private GameObject CreateLine(Vector2 start, Vector2 end)
    {
        GameObject obj = new GameObject("Line", typeof(Image));
        obj.transform.SetParent(chartContainer, false);
        Image img = obj.GetComponent<Image>();
        img.color = Color.green;

        RectTransform rt = obj.GetComponent<RectTransform>();
        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.sizeDelta = new Vector2(distance, 3);
        rt.anchoredPosition = start + dir * distance * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        return obj;
    }

    private GameObject CreateVerticalLine(float x, float height)
    {
        GameObject obj = new GameObject("VerticalLine", typeof(Image));
        obj.transform.SetParent(chartContainer, false);
        Image img = obj.GetComponent<Image>();
        img.color = Color.white;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.sizeDelta = new Vector2(2, height);
        rt.anchoredPosition = new Vector2(x, height / 2);

        return obj;
    }

    private void ClearDynamicChart()
    {
        foreach (var obj in chartObjects)
            Destroy(obj);
        chartObjects.Clear();
        pointRects.Clear();
    }
}
