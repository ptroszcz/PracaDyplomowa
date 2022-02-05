using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Quantity
{
    public int min;

    public int max;

    public Quantity(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public int Random
    {
        get
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
