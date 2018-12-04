using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexTerrain : MonoBehaviour {

    public float terrainScale = 5.0f;
    public float forestScale = 5.0f;
    public int chunkSize = 8;
    public int numChunks = 4;
    public int maxHeight = 6;
    public int waterLevel = 2;

    public float treeChance = 0.5f;

    public Text cellLabelPrefab;

    TerrainChunk chunkTemplate;
    TerrainChunk[,] chunks;

    // Use this for initialization
    void Awake() {
        chunkTemplate = GetComponentInChildren<TerrainChunk>();
        chunkTemplate.terrain = this;
        chunkTemplate.size = chunkSize;

        transform.position -= new Vector3(GetWidth() / 2 - HexMetrics.innerRadius, 0, GetDepth() / 2 - (HexMetrics.outerRadius * 0.75f));

        GenerateProceduralMap();
    }

    private void GenerateProceduralMap() {
        chunks = new TerrainChunk[numChunks, numChunks];

        int rows = chunkSize * numChunks;
        int columns = chunkSize * numChunks;

        float[,] heightMap = GameUtils.GenerateNoiseMap(rows, columns, terrainScale);
        float[,] treeMap = GameUtils.GenerateNoiseMap(rows, columns, forestScale);

        for (int z = 0, i = 0; z < numChunks; z++) {
            for (int x = 0; x < numChunks; x++, i++) {

                int subsetOriginX = x * chunkSize;
                int subsetOriginZ = z * chunkSize;
                int subsetEndX = subsetOriginX + chunkSize;
                int subsetEndZ = subsetOriginZ + chunkSize;

                float[,] heightMapSubset = GameUtils.Get2DArraySubset(heightMap, new Vector2Int(subsetOriginX, subsetOriginZ), new Vector2Int(subsetEndX, subsetEndZ));
                float[,] treeMapSubset = GameUtils.Get2DArraySubset(treeMap, new Vector2Int(subsetOriginX, subsetOriginZ), new Vector2Int(subsetEndX, subsetEndZ));
                chunks[x, z] = Instantiate<TerrainChunk>(chunkTemplate);
                chunks[x, z].name = "Chunk" + i;
                chunks[x, z].Init(this, heightMapSubset, treeMapSubset, new Vector2Int(subsetOriginX, subsetOriginZ));
            }
        }
    }

    public TerrainChunk GetChunkFromWorldCoords(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();

        Vector2 chunkIndex = new Vector2(offsetCoords.x / chunkSize, offsetCoords.y / chunkSize);

        TerrainChunk chunk = null;
        try {
            chunk = chunks[(int)Math.Floor(chunkIndex.x), (int)Math.Floor(chunkIndex.y)];
        }
        catch (IndexOutOfRangeException e) {
            Debug.LogWarning("WARNING: Tried to access chunk that is out of bounds");
        }
        return chunk;
    }

    public float GetWidth() {
        float unscaled = chunkSize * numChunks * (HexMetrics.innerRadius * 2) + HexMetrics.innerRadius;
        return unscaled * transform.localScale.x;
    }

    public float GetDepth() {
        float unscaled = chunkSize * numChunks * HexMetrics.outerRadius * 1.5f;
        return unscaled * transform.localScale.z;
    }
}
