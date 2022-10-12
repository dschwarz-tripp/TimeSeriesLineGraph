using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DataPoint
{
    public DateTime TimeStamp { get; set; }
    public float Value { get; set; }
}

public class LineGraph : MonoBehaviour
{
    [Header("Graph Objects")]
    [SerializeField]
    private RectTransform graphContainer;

    [SerializeField]
    private RectTransform tempValueX, tempValueY;

    [SerializeField]
    private RectTransform tempGridlineX, tempGridlineY;

    private float graphWidth, graphHeight;
    
    [Header("Graph Data")]
    [SerializeField]
    private Sprite dataPointSprite;
    [SerializeField]
    private Vector2 dataPointSize = new Vector2(15f, 15f);

    [SerializeField]
    private List<DataPoint> dataToGraphList = new List<DataPoint>();

   
    [Header("Axes Values")]
    [SerializeField]
    private TimeAxis xAxis;
    [SerializeField]
    private YAxis yAxis;
    [SerializeField]
    private int timeIntervals = 10;


    [SerializeField]
    private bool plotGridlines;

    private List<GameObject> graphedObjList = new List<GameObject>();


    [Space, SerializeField]
    private Color connectorColor = new Color(0, 0, 0, 0.25f);
    [SerializeField]
    private float connectorWidth = 2f;

    private Vector2 defaultVector = Vector2.zero;
    private Vector3 defaultScale = Vector3.one;

    private void OnEnable()
    {
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;

        var fakeData = new List<DataPoint>();
        for(int i = 0; i <= 50; i++)
        {
            fakeData.Add( new DataPoint { Value = UnityEngine.Random.Range(1, 20), TimeStamp = DateTime.Now + new TimeSpan(0, i, 0) });
        }

        GraphDataSeries(fakeData);
    }

    private void GraphDataSeries(List<DataPoint> dataSeries)
    {
        graphedObjList.ForEach(obj => Destroy(obj));
        graphedObjList.Clear();

        if(dataSeries.Count <= 2)
        {
            Debug.LogError("Cannot plot charts with only two data points");
            return;
        }

        dataToGraphList = dataSeries.OrderBy(data => data.TimeStamp).ToList();

        SetXAxisMinMax(dataToGraphList);

        SetYAxisMinMax(dataToGraphList);

        PlotXAxisLabels();

        PlotYAxisLabels();

        PlotDataPoints(dataToGraphList);
    }

    private void SetXAxisMinMax(List<DataPoint> dataSeries)
    {
        xAxis.MinDateTime = dataSeries.First().TimeStamp;
        xAxis.MaxDateTime = dataSeries.Last().TimeStamp; 
    }

    private void SetYAxisMinMax(List<DataPoint> dataSeries)
    {
        yAxis.MinValue = dataSeries.Min(x => x.Value);
        yAxis.MaxValue = dataSeries.Max(x => x.Value);

        yAxis.MinValue -= (yAxis.ValueRange * yAxis.spacing);
        yAxis.MaxValue += (yAxis.ValueRange * yAxis.spacing);
    }

    private void PlotXAxisLabels()
    {        
        xAxis.labelCount = Mathf.RoundToInt((float)(xAxis.MaxDateTime - xAxis.MinDateTime).TotalMinutes / this.timeIntervals) + 1;

        float currentLabelPosition;
        float labelIndex = 0;
        float labelWidth = graphWidth / (xAxis.labelCount + xAxis.spacing);

        for (int i = 0; i < xAxis.labelCount; i++)
        {
            // Labels
            currentLabelPosition = labelWidth + i * labelWidth;

            RectTransform labelRect = Instantiate(tempValueX);
            labelRect.SetParent(graphContainer);
            labelRect.gameObject.SetActive(true);
            labelRect.anchoredPosition = new Vector2(currentLabelPosition, labelRect.position.y);
            labelRect.GetComponent<TextMeshProUGUI>().text = $"{i*timeIntervals} minutes";
            labelRect.localScale = defaultScale;

            graphedObjList.Add(labelRect.gameObject);

            // Gridlines
            if (plotGridlines)
            {
                RectTransform gridLineRect = Instantiate(tempGridlineX);
                gridLineRect.SetParent(graphContainer);
                gridLineRect.gameObject.SetActive(true);
                gridLineRect.anchoredPosition = new Vector2(currentLabelPosition, gridLineRect.position.y);
                gridLineRect.localScale = defaultScale;

                graphedObjList.Add(gridLineRect.gameObject);
            }
            
            labelIndex++;
        }
    }

    private void PlotYAxisLabels()
    {
        float tempLabelCount = yAxis.labelCount;
        float labelPosition, labelPositionNormal;

        if (tempLabelCount > yAxis.ValueRange)
        {
            int addTo(int to)
            {
                return (to % 2 == 0) ? to : (to + 2);
            }

            if (yAxis.ValueRange % 2 != 0)
            {
                tempLabelCount = addTo((int)yAxis.ValueRange);
            }
            else
            {
                tempLabelCount = (int)yAxis.ValueRange;
            }

            if (yAxis.ValueRange == 1)
            {
                tempLabelCount = Mathf.RoundToInt(yAxis.ValueRange) + 3;
                yAxis.MinValue -= 2;
                yAxis.MaxValue += 2;
            }
        }

        for (int i = 0; i <= tempLabelCount; i++)
        {
            labelPositionNormal = (i * 1f) / tempLabelCount;

            labelPosition = yAxis.MinValue + (labelPositionNormal * (yAxis.MaxValue - yAxis.MinValue));

            // Labels

            var labelRect = Instantiate(tempValueY);
            labelRect.SetParent(graphContainer);
            labelRect.gameObject.SetActive(true);
            labelRect.anchoredPosition = new Vector2(labelRect.position.x, labelPositionNormal * graphHeight);
            labelRect.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(labelPosition).ToString();
            labelRect.localScale = defaultScale;

            graphedObjList.Add(labelRect.gameObject);

            if (plotGridlines)
            {
                // Gridlines
                if (i != 0 && i != tempLabelCount)
                {

                    var gridlineRect = Instantiate(tempGridlineY);
                    gridlineRect.SetParent(graphContainer);
                    gridlineRect.gameObject.SetActive(true);
                    gridlineRect.anchoredPosition = new Vector2(gridlineRect.position.x, labelPositionNormal * graphHeight);
                    gridlineRect.localScale = defaultScale;

                    graphedObjList.Add(gridlineRect.gameObject);
                }
            }
        }
    }

    private void PlotDataPoints(List<DataPoint> dataSeries)
    {
        GameObject lastDataPoint = null;

        var labelWidth = graphWidth / (xAxis.labelCount + xAxis.spacing);
        var minXLabelPosition = labelWidth;
        var maxXLabelPosition = xAxis.labelCount * labelWidth;

        for (int i = 0; i < dataSeries.Count; i++)
        {
            var totalMinutes = (float)xAxis.TotalTimeSpan.TotalMinutes;

            var dataPointMinutes = (float)(dataSeries[i].TimeStamp - xAxis.MinDateTime).TotalMinutes;

            var xAxisLabelRange = maxXLabelPosition - minXLabelPosition;

            var xAxisGraphPosition = (dataPointMinutes / totalMinutes) * xAxisLabelRange + minXLabelPosition;
            var yAxisGraphPosition = (dataSeries[i].Value - yAxis.MinValue) / yAxis.ValueRange * graphHeight;

            var dataPosition = new Vector2(xAxisGraphPosition, yAxisGraphPosition);

            var newDataPoint = CreateDataPoint(dataPosition);

            graphedObjList.Add(newDataPoint);

            if (lastDataPoint != null)
            {
                var dataConnector = CreateDataConnector(lastDataPoint.GetComponent<RectTransform>().anchoredPosition, newDataPoint.GetComponent<RectTransform>().anchoredPosition);
                graphedObjList.Add(dataConnector);
            }
            
            lastDataPoint = newDataPoint;
        }
    }

    private GameObject CreateDataPoint(Vector2 pos)
    {
        var dataPointObj = new GameObject("Data", typeof(Image));
        dataPointObj.transform.SetParent(graphContainer, false);
        dataPointObj.GetComponent<Image>().sprite = dataPointSprite;
        
        var dataPointRect = dataPointObj.GetComponent<RectTransform>();
        dataPointRect.anchoredPosition = pos;
        dataPointRect.sizeDelta = dataPointSize;
        dataPointRect.anchorMax = defaultVector;
        dataPointRect.anchorMin = defaultVector;

        return dataPointObj;
    }

    private GameObject CreateDataConnector(Vector2 pointA, Vector2 pointB)
    {
        var connectorObj = new GameObject("Connection", typeof(Image));
        connectorObj.transform.SetParent(graphContainer, false);
        connectorObj.GetComponent<Image>().color = connectorColor;

        var connectorDirection = (pointB - pointA).normalized;

        var connectorDistance = Vector2.Distance(pointA, pointB);

        var connectorAngle = Mathf.Atan2(connectorDirection.y, connectorDirection.x) * Mathf.Rad2Deg;

        var connectorRect = connectorObj.GetComponent<RectTransform>();
        connectorRect.anchoredPosition = pointA + connectorDirection * connectorDistance * 0.5f;
        connectorRect.sizeDelta = new Vector2(connectorDistance, connectorWidth);
        connectorRect.anchorMin = defaultVector;
        connectorRect.anchorMax = defaultVector;
        connectorRect.localEulerAngles = new Vector3(0, 0, connectorAngle);

        return connectorObj;
    }
}


