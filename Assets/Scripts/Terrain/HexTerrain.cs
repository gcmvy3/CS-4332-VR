using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexTerrain : MonoBehaviour {

    public float terrainScale = 5.0f;
    public int chunkSize = 8;
    public int chunkRows = 4;
    public int chunkColumns = 4;
    public int maxHeight = 6;
    public int waterLevel = 2;

    public int pedestalHeight = 5;
    public int pedestalRimSize = 10;

    public float treeChance = 0.5f;

    public Text cellLabelPrefab;

    public GameObject pedestal;

    TerrainChunk chunkTemplate;
    TerrainChunk[,] chunks;

    // Use this for initialization
    void Awake() {
        chunkTemplate = GetComponentInChildren<TerrainChunk>();
        chunkTemplate.terrain = this;
        chunkTemplate.size = chunkSize;

        Vector3 centerOffset = new Vector3(chunkSize * HexMetrics.innerRadius * chunkColumns, 0, 0);
        transform.position -= centerOffset;

        InitPedestal();
        GenerateProceduralMap();
    }

    private void InitPedestal() {
        pedestal = GameObject.Instantiate(GameObject.Find("Cloneables/Pedestal"));

        float pedestalWidth = chunkSize * chunkColumns * (HexMetrics.innerRadius * 2) + HexMetrics.innerRadius + pedestalRimSize * 2;
        float pedestalDepth = chunkSize * chunkRows * HexMetrics.outerRadius * 1.5f + pedestalRimSize * 2;

        GameUtils.ScaleGameObjectToSize(pedestal, new Vector3(pedestalWidth, pedestalHeight, pedestalDepth));

        float pedestalX = transform.position.x + pedestalWidth / 2 - HexMetrics.innerRadius - pedestalRimSize;
        float pedestalZ = transform.position.z + pedestalDepth / 2 - HexMetrics.outerRadius - pedestalRimSize; 
        pedestal.transform.position = new Vector3(pedestalX, transform.position.y - pedestalHeight, pedestalZ);
    }

    private void GenerateProceduralMap() {
        chunks = new TerrainChunk[chunkColumns, chunkRows];

        int rows = chunkSize * chunkRows;
        int columns = chunkSize * chunkColumns;

        float[,] heightMap = GameUtils.GenerateNoiseMap(rows, columns, terrainScale);

        for (int z = 0, i = 0; z < chunkRows; z++) {
            for (int x = 0; x < chunkColumns; x++, i++) {

                int subsetOriginX = x * chunkSize;
                int subsetOriginZ = z * chunkSize;
                int subsetEndX = subsetOriginX + chunkSize;
                int subsetEndZ = subsetOriginZ + chunkSize;

                float[,] heightMapSubset = GameUtils.Get2DArraySubset(heightMap, new Vector2(subsetOriginX, subsetOriginZ), new Vector2(subsetEndX, subsetEndZ));
                chunks[x, z] = Instantiate<TerrainChunk>(chunkTemplate);
                chunks[x, z].name = "Chunk" + i;
                chunks[x, z].Init(this, heightMapSubset, new Vector2(subsetOriginX, subsetOriginZ));
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
}
