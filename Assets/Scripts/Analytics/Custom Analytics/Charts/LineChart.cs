using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a line chart visualization of skill performance data.
/// Updates dynamically based on user performance and saved chart history.
/// </summary>
public class LineChart : MonoBehaviour
{  
    /// <summary>
    /// Defines the type of skill data to display on each axis.
    /// </summary>
    public enum SkillDataType { Attempts, Successes, SuccessRate }

    public SkillDataType dataTypeXaxis;
    public SkillDataType dataTypeYaxis;

    public RectTransform chartContainer;
    public Sprite circleSprite;

    private List<GameObject> chartObjects = new();
    private List<RectTransform> pointRects = new();

    private List<SkillChartData> skillChartData = new();
      
    /// <summary>
    /// Loads saved skill chart data when a skill is selected or changed.
    /// </summary>
    /// <param name="skill">The selected skill.</param>
    public async void LoadChartData(string filename)
    {
        SkillChartDataWrapper chartData = await ChartInfo.LoadAllTimeChartAsync(filename);
        skillChartData = new(chartData.history);

        DisplaySavedData();
    }

    /// <summary>
    /// Displays saved chart data for a given skill.
    /// </summary>
    private void DisplaySavedData()
    {
        List<float> xValues = new();
        List<float> yValues = new();

        for (int i = 0; i < skillChartData.Count; i++)
        {
            var s = skillChartData[i];

            float x = dataTypeXaxis switch
            {
                SkillDataType.Attempts => s.attempts,
                SkillDataType.Successes => s.successes,
                SkillDataType.SuccessRate => s.attempts == 0 ? 0f : (float)s.successes / s.attempts * 100f,
                _ => 0f
            };

            float y = dataTypeYaxis switch
            {
                SkillDataType.Attempts => s.attempts,
                SkillDataType.Successes => s.successes,
                SkillDataType.SuccessRate => s.attempts == 0 ? 0f : (float)s.successes / s.attempts * 100f,
                _ => 0f
            };

            xValues.Add(x);
            yValues.Add(y);
        }

        ShowGraph(xValues, yValues);
    }

    /// <summary>
    /// Displays the line chart based on X and Y value lists.
    /// </summary>
    /// <param name="xValues">List of X-axis values.</param>
    /// <param name="yValues">List of Y-axis values.</param>
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

    /// <summary>
    /// Draws vertical axis labels for the X-axis.
    /// </summary>
    /// <param name="maxX">Maximum X value for scaling.</param>
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

    /// <summary>
    /// Draws horizontal grid lines and Y-axis labels.
    /// </summary>
    /// <param name="maxY">Maximum Y value for scaling.</param>
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

    /// <summary>
    /// Creates a UI text label at the specified position.
    /// </summary>
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

    /// <summary>
    /// Creates a horizontal or vertical line UI element.
    /// </summary>
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

    /// <summary>
    /// Creates a point (circle) at the specified chart position.
    /// </summary>
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

    /// <summary>
    /// Creates a line connecting two chart points.
    /// </summary>
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

    /// <summary>
    /// Creates a vertical guideline at a specified X position.
    /// </summary>
    private GameObject CreateVerticalLine(float x, float height)
    {
        return CreateLineObject(new Vector2(2, height), new Vector2(x, height / 2));
    }

    /// <summary>
    /// Clears all dynamically created chart elements from the display.
    /// </summary>
    private void ClearDynamicChart()
    {
        foreach (var obj in chartObjects)
            Destroy(obj);

        chartObjects.Clear();
        pointRects.Clear();
    }
}
