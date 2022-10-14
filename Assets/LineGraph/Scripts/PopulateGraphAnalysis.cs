using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class PopulateGraphAnalysis : MonoBehaviour
{
    // Start is called before the first frame update

    public TextAsset sessionCSV;

    public LineGraph hrvGraph;
    public LineGraph hrGraph;
    [SerializeField]
    private TextMeshProUGUI SummaryAverage;
    [SerializeField]
    private TextMeshProUGUI SummaryFocus;
    [SerializeField]
    private TextMeshProUGUI SummaryTime;

    void Start()
    {

    }

    private void GraphTestData()
    {
        if (sessionCSV != null)
        {
            var lines = sessionCSV.text.Split(new[] { '\r', '\n' });
            var hrData = new List<DataPoint>();
            var hrvData = new List<DataPoint>();
            //skip the header
            //acuracy, timestamp, type, value

            var values = new List<float>();
            int plotSegments = 40;
            int idx = 0;
            foreach (var line in lines.Skip(1))
            {
                var csv = line.Split(',');

                if (csv.Length < 3)
                {
                    continue;
                }

                if (csv[2] == "HeartRate")
                {
                    values.Add(float.Parse(csv[3]));
                    if (idx % plotSegments == 0)
                    {
                        hrData.Add(new DataPoint { TimeStamp = TimeStampToDateTime(long.Parse(csv[1])), Value = values.Average() });
                        values.Clear();
                    }

                }

                idx++;
            }


            //label it too for testing

            var third = (hrData.Max(x => x.TimeStamp) - hrData.Min(x => x.TimeStamp)).TotalMinutes / 3;
            var first = hrData.Min(x => x.TimeStamp).AddMinutes(third);
            var second = first.AddMinutes(third);

            foreach (var point in hrData)
            {
                if (point.TimeStamp < first)
                {
                    hrvData.Add(new DataPoint { TimeStamp = point.TimeStamp, Value = UnityEngine.Random.Range(10, 20), Label = "A" });

                    point.Label = "A";
                }
                else if (point.TimeStamp > first && point.TimeStamp < second)
                {
                    hrvData.Add(new DataPoint { TimeStamp = point.TimeStamp, Value = UnityEngine.Random.Range(30, 80), Label = "B" });
                    point.Label = "B";
                }
                else
                {
                    hrvData.Add(new DataPoint { TimeStamp = point.TimeStamp, Value = UnityEngine.Random.Range(20, 40), Label = "C" });
                    point.Label = "C";
                }
            }



            if (hrGraph != null)
            {
                hrGraph.GraphDataSeries(hrData);
            }

            if (hrvGraph != null)
            {
                hrvGraph.GraphDataSeries(hrvData);
            }

            SetSummaryText(hrData);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public static DateTime TimeStampToDateTime(long timeStamp)
    {
        // Java timestamp is milliseconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds(timeStamp/1e9);
        return dateTime;
    }

    private void SetSummaryText(List<DataPoint> dataSeries)
    {
        if (SummaryAverage != null)
        {
            SummaryAverage.text = dataSeries.Select(x => x.Value).Average().ToString("F2") + " bpm";
        }

        if (SummaryFocus != null)
        {
            //calculate or acquire % focus?
        }

        if (SummaryTime != null)
        {
            var totalTime = dataSeries.Max(x=>x.TimeStamp) - dataSeries.Min(x=>x.TimeStamp);
            SummaryTime.text = totalTime.ToString(@"mm\:ss") + " min";
        }
    }
}
