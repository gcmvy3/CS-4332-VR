﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

    [SerializeField]
    private int x, z;

    public HexCoordinates(int x, int z) {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates (int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public int X {
        get
        {
            return x;
        }
    }
    public int Z {
        get
        {
            return z;
        }
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
    }
}
