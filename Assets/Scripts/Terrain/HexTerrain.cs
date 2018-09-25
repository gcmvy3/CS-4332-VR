using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexTerrain : MonoBehaviour {

    public static float terrainScale = 5.0f;
    public int chunkSize = 16;
    public int chunkRows = 2;
    public int chunkColumns = 2;
    public int maxHeight = 6;
    public int waterLevel = 2;
    public bool showCoordinates = true;

    public Text cellLabelPrefab;

    TerrainChunk chunkTemplate;
    TerrainChunk[,] chunks;

    // Use this for initialization
    void Awake() {
        chunkTemplate = GetComponentInChildren<TerrainChunk>();
        chunkTemplate.terrain = this;
        chunkTemplate.size = chunkSize;

        GenerateProceduralMap();
    }

    public void GenerateProceduralMap() {
        chunks = new TerrainChunk[chunkColumns, chunkRows];

        int rows = chunkSize * chunkRows;
        int columns = chunkSize * chunkColumns;

        float[,] heightMap = Utils.GenerateNoiseMap(rows, columns, terrainScale);

        for (int z = 0, i = 0; z < chunkRows; z++) {
            for (int x = 0; x < chunkColumns; x++, i++) {

                int subsetOriginX = x * chunkSize;
                int subsetOriginZ = z * chunkSize;
                int subsetEndX = subsetOriginX + chunkSize;
                int subsetEndZ = subsetOriginZ + chunkSize;

                float[,] heightMapSubset = Utils.Get2DArraySubset(heightMap, new Vector2(subsetOriginX, subsetOriginZ), new Vector2(subsetEndX, subsetEndZ));
                chunks[x, z] = Instantiate<TerrainChunk>(chunkTemplate);
                chunks[x, z].name = "Chunk" + i;
                chunks[x, z].Init(this, heightMapSubset, new Vector2(subsetOriginX, subsetOriginZ));
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public TerrainChunk GetChunk(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();

        Vector2 chunkCoords = new Vector2(offsetCoords.x / chunkSize, offsetCoords.y / chunkSize);

        TerrainChunk chunk = null;
        try {
            chunk = chunks[(int)chunkCoords.x, (int)chunkCoords.y];
        }
        catch (IndexOutOfRangeException e) {
            Debug.LogWarning("WARNING: Tried to access chunk that is out of bounds");
        }
        return chunk;
    }

    public CellStack GetCellStack(HexCoordinates coords) {
        CellStack stack = null;
        try {
            TerrainChunk chunk = GetChunk(coords);
            stack = chunk.GetCellStack(coords);
        }
        catch (IndexOutOfRangeException e) {
            Debug.LogWarning("WARNING: Tried to access CellStack that is out of bounds");
        }
        return stack;
    }

    public CellStack GetCellStack(Vector2 coords) {
        Vector2 chunkCoords = new Vector2(coords.x / chunkSize, coords.y / chunkSize);

        CellStack stack = null;
        try {
            TerrainChunk chunk = chunks[(int)chunkCoords.x, (int)chunkCoords.y];
            stack = chunk.GetCellStack(coords);
        }
        catch (IndexOutOfRangeException e) {
            Debug.LogWarning("WARNING: Tried to access CellStack that is out of bounds");
        }
        return stack;
    }
}
