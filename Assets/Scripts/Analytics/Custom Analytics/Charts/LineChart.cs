using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChart : MonoBehaviour
{
    public enum SkillDataType { Attempts, Successes, SuccessRate }

    public SkillDataType dataTypeXaxis;
    public SkillDataType dataTypeYaxis;

    public RectTransform chartContainer;
    public Sprite circleSprite;

    private List<GameObject> chartObjects = new();
    private List<RectTransform> pointRects = new();
    private List<SkillAnalyzeData> skillAnalyzeHistory = new();

    private List<SkillChartData> skillChartData = new();

    private void Awake()
    {
        EventManager.OnSkillChanged.AddListener(LoadChartData);
    }


    void Start() => DisplaySavedData(SkillMovesManager.CurrentSkill);

    private void OnDisable() => EventManager.OnSkillChanged.RemoveListener(LoadChartData);

    void LoadChartData(Skill skill)
    {
        SkillChartDataWrapper chartData = SaveChartInfo.LoadChart(skill);
        skillChartData = new(chartData.history);

        DisplaySavedData(skill);
    }

    private void ReceiveData(SkillAnalyzeData skillData)
    {
        skillAnalyzeHistory.Clear();

        skillAnalyzeHistory.Add(skillData);
        if (skillAnalyzeHistory.Count > 10) skillAnalyzeHistory.RemoveAt(0);

        List<float> xValues = new();
        List<float> yValues = new();

        foreach (var s in skillChartData)
        {
            float x = dataTypeXaxis switch
            {
                SkillDataType.Attempts => s.attempts,
                SkillDataType.Successes => s.success,
                SkillDataType.SuccessRate => s.attempts == 0 ? 0f : (float)s.success / s.attempts * 100f,
                _ => 0f
            };

            float y = dataTypeYaxis switch
            {
                SkillDataType.Attempts => s.attempts,
                SkillDataType.Successes => s.success,
                SkillDataType.SuccessRate => s.attempts == 0 ? 0f : (float)s.success / s.attempts * 100f,
                _ => 0f
            };
        }

        ShowGraph(xValues, yValues);
    }

    private void DisplaySavedData(Skill skill)
    {
        List<float> xValues = new();
        List<float> yValues = new();

        for (int i = 0; i < skillChartData.Count; i++)
        {
            var s = skillChartData[i];

            float x = dataTypeXaxis switch
            {
                SkillDataType.Attempts => s.attempts,
                SkillDataType.Successes => s.success,
                SkillDataType.SuccessRate => s.attempts == 0 ? 0f : (float)s.success / s.attempts * 100f,
                _ => 0f
            };

            float y = dataTypeYaxis switch
            {
                SkillDataType.Attempts => s.attempts,
                SkillDataType.Successes => s.success,
                SkillDataType.SuccessRate => s.attempts == 0 ? 0f : (float)s.success / s.attempts * 100f,
                _ => 0f
            };

            xValues.Add(x);
            yValues.Add(y);
        }

        ShowGraph(xValues, yValues);
    }

    public void ShowGraph(List<float> xValues, List<float> yValues)
    {
        if (xValues.Count <= 0 || yValues.Count <= 0)
        {
            Debug.Log("Nothing to display");
            return;
        }

        ClearDynamicChart();

        float chartWidth = chartContainer.sizeDelta.x;
        float chartHeight = chartContainer.sizeDelta.y;

        float maxY = Mathf.Max(1f, Mathf.Max(yValues.ToArray()));
        float maxX = Mathf.Max(1f, Mathf.Max(xValues.ToArray()));
        float yMaxRounded = Mathf.Ceil(maxY / 10f) * 10f;
        float xMaxRounded = Mathf.Ceil(maxX / 5f) * 5f;

        DrawHorizontalLines(yMaxRounded);
        DrawVerticalLabels(xMaxRounded);

        float firstX = (xValues[0] / xMaxRounded) * chartWidth;
        float firstY = (yValues[0] / yMaxRounded) * chartHeight;

        Vector2 startPos = new(0, 0);
        Vector2 firstPointPos = new(firstX, firstY);

        chartObjects.Add(CreateLine(startPos, firstPointPos));

        for (int i = 0; i < xValues.Count; i++)
        {
            float x = (xValues[i] / xMaxRounded) * chartWidth;
            float y = (yValues[i] / yMaxRounded) * chartHeight;
            Vector2 currentPos = new(x, y);

            var point = CreatePoint(currentPos);
            chartObjects.Add(point);
            pointRects.Add(point.GetComponent<RectTransform>());

            chartObjects.Add(CreateVerticalLine(x, chartHeight));

            if (i > 0)
            {
                Vector2 prevPos = pointRects[i - 1].anchoredPosition;
                chartObjects.Add(CreateLine(prevPos, currentPos));
            }
        }
    }

    private void DrawVerticalLabels(float maxX)
    {
        int steps = 5;
        float width = chartContainer.sizeDelta.x;

        for (int i = 0; i <= steps; i++)
        {
            float normalized = i / (float)steps;
            float xPos = normalized * width;

            chartObjects.Add(CreateTextLabel((normalized * maxX).ToString("F0"), new Vector2(xPos, -20)));
        }
    }

    //every time the skill is gonne be performed, it saves how many times attempt this session, and save overall successes, and it updates the chart based on that
    private void DrawHorizontalLines(float maxY)
    {
        int steps = 5;
        float width = chartContainer.sizeDelta.x;
        float height = chartContainer.sizeDelta.y;

        for (int i = 0; i <= steps; i++)
        {
            float normalized = i / (float)steps;
            float yPos = normalized * height;

            chartObjects.Add(CreateLineObject(new Vector2(width, 2), new Vector2(width / 2, yPos)));
            chartObjects.Add(CreateTextLabel((normalized * maxY).ToString("F0"), new Vector2(-35, yPos)));
        }
    }

    private GameObject CreateTextLabel(string textValue, Vector2 position)
    {
        GameObject label = new("Label", typeof(Text));
        label.transform.SetParent(chartContainer, false);
        Text text = label.GetComponent<Text>();
        text.text = textValue;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;

        RectTransform rt = text.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(60, 20);

        return label;
    }

    private GameObject CreateLineObject(Vector2 size, Vector2 position)
    {
        GameObject line = new("Line", typeof(Image));
        line.transform.SetParent(chartContainer, false);
        line.GetComponent<Image>().color = Color.white;

        RectTransform rt = line.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.sizeDelta = size;
        rt.anchoredPosition = position;

        return line;
    }

    private GameObject CreatePoint(Vector2 position)
    {
        GameObject point = new("Point", typeof(Image));
        point.transform.SetParent(chartContainer, false);
        point.GetComponent<Image>().sprite = circleSprite;

        RectTransform rt = point.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(15, 15);
        rt.anchorMin = rt.anchorMax = Vector2.zero;

        return point;
    }

    private GameObject CreateLine(Vector2 start, Vector2 end)
    {
        GameObject line = new("Line", typeof(Image));
        line.transform.SetParent(chartContainer, false);
        Image img = line.GetComponent<Image>();
        img.color = Color.green;

        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        RectTransform rt = line.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.sizeDelta = new Vector2(distance, 3);
        rt.anchoredPosition = start + dir * distance * 0.5f;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        return line;
    }

    private GameObject CreateVerticalLine(float x, float height)
    {
        return CreateLineObject(new Vector2(2, height), new Vector2(x, height / 2));
    }

    private void ClearDynamicChart()
    {
        foreach (var obj in chartObjects)
            Destroy(obj);

        chartObjects.Clear();
        pointRects.Clear();
    }
}
