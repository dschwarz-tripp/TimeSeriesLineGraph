using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XAxis
{
    public int labelCount = 6;
    public float spacing = 1f;

    public DateTime MinDateTime { get; set; }
    public DateTime MaxDateTime { get; set; }
    public TimeSpan TotalTime => MaxDateTime - MinDateTime;
    public float MinLabelPos { get; set; }
    public float MaxLabelPos { get; set; }
}
