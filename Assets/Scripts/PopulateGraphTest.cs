using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateGraphTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var graph = GetComponent<LineGraph>();

        var fakeData = new List<DataPoint>();
        for (int i = 0; i <= 50; i++)
        {
            fakeData.Add(new DataPoint { Value = UnityEngine.Random.Range(1, 20), TimeStamp = DateTime.Now + new TimeSpan(0, i, 0) });
        }

        graph.GraphDataSeries(fakeData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
