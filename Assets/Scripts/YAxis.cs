using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class YAxis 
{
    public int labelCount = 10;
    public float gridlineWidth = 2f;
    public float spacing = 0.2f;

    public float defaultMinValue = 75;
    public float defaultMaxValue = 150;

    public float MinValue { get;set; }
    public float MaxValue { get; set; }
    public float ValueRange => MaxValue - MinValue; 
}
