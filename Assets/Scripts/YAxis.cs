using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class YAxis 
{
    public int labelCount = 10;
    public float spacing = 0.2f;

    public float MinValue { get; set; } = 75;
    public float MaxValue { get; set; } = 150;
    public float ValueRange => MaxValue - MinValue; 
}
