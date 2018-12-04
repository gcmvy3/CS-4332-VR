using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObject : MonoBehaviour {

    public HexCoordinates coordinates;
    public Vector2 indexWithinChunk;

    bool initialized = false;

    public void Init(HexCoordinates coords, Vector2 indexInChunk) {
        coordinates = coords;
        indexWithinChunk = indexInChunk;
        initialized = true;
    }
}
