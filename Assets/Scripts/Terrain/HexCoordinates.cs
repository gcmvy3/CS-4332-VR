using System.Collections;
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

    public static HexCoordinates WorldCoordsFromGlobalPosition(HexTerrain terrain, Vector3 position) {

        position -= terrain.transform.position;

        float x = position.x / ((HexMetrics.innerRadius * terrain.transform.localScale.x) * 2f);
        float y = -x;

        float offset = position.z / ((HexMetrics.outerRadius * terrain.transform.localScale.z) * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);

        if (iX + iY + iZ != 0) {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);

            if (dX > dY && dX > dZ) {
                iX = -iY - iZ;
            }
            else if (dZ > dY) {
                iZ = -iX - iY;
            }
        }

        return new HexCoordinates(iX, iZ);
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

    public Vector3 ToChunkPosition() {
        Vector2 offset = ToOffsetCoordinates();

        Vector3 position = new Vector3();
        position.x = offset.x * (HexMetrics.innerRadius * 2f) + (offset.y % 2) * HexMetrics.innerRadius; ;
        position.y = 0f;
        position.z = offset.y * (HexMetrics.outerRadius * 1.5f);

        return position;
    }

    public Vector3 ToWorldPosition(HexTerrain terrain) {

        Vector2 offset = ToOffsetCoordinates();

        Vector3 position = new Vector3();
        position.x = offset.x * (HexMetrics.ScaledInnerRadius(terrain) * 2f) + (offset.y % 2) * HexMetrics.ScaledInnerRadius(terrain); ;
        position.y = 0f;
        position.z = offset.y * (HexMetrics.ScaledOuterRadius(terrain) * 1.5f);

        position += terrain.transform.position;

        return position;
    }

    public Vector2 ToOffsetCoordinates() {
        int col = x + (z - (z & 1)) / 2;
        int row = z;

        return new Vector2(col, row);
    }

    public HexCoordinates[] GetNeighbors() {
        //Clockwise starting at the north neighbor

        HexCoordinates[] neighbors = new HexCoordinates[6];
        neighbors[0] = new HexCoordinates(X, Z + 1);
        neighbors[1] = new HexCoordinates(X + 1, Z);
        neighbors[2] = new HexCoordinates(X + 1, Z - 1);
        neighbors[3] = new HexCoordinates(X, Z - 1);
        neighbors[4] = new HexCoordinates(X - 1, Z);
        neighbors[5] = new HexCoordinates(X - 1, Z + 1);

        return neighbors;
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
