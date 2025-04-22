using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChart : MonoBehaviour
{
    public RectTransform chartContainer;
    public Sprite circleSprite;
    private List<GameObject> chartObjects = new();

    private void Start()
    {
        List<float> exampleData = new List<float> { 10, 25, 50, 40, 60, 70, 90, 100,15, 37 };
        ShowGraph(exampleData);
    }

    public void ShowGraph(List<float> values)
    {
        ClearChart();

        float chartHeight = chartContainer.sizeDelta.y;
        float chartWidth = chartContainer.sizeDelta.x;
        float xSpacing = chartWidth / (values.Count - 1);

        for (int i = 0; i < values.Count; i++)
        {
            float x = i * xSpacing;
            float y = values[i] / 100f * chartHeight; // normalize values

            var point = CreatePoint(new Vector2(x, y));
            chartObjects.Add(point);

            if (i > 0)
            {
                var prevPos = chartObjects[chartObjects.Count - 2].GetComponent<RectTransform>().anchoredPosition;
                //var line = CreateLine(prevPos, new Vector2(x, y));
                //chartObjects.Add(line);
            }
        }
    }

    private GameObject CreatePoint(Vector2 position)
    {
        GameObject obj = new GameObject("Point", typeof(Image));
        obj.transform.SetParent(chartContainer, false);
        obj.GetComponent<Image>().sprite = circleSprite;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(11, 11);
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

    private void ClearChart()
    {
        foreach (var obj in chartObjects)
            Destroy(obj);
        chartObjects.Clear();
    }
}
