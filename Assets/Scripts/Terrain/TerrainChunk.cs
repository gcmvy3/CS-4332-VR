using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainChunk : MonoBehaviour {

    public bool showCoordinates = false;
    public HexTerrain terrain;
    public int size;

    bool initialized = false;
    
    CellStack[,] cellStacks;

    TerrainCollisionMesh collisionMesh;
    TerrainRenderMesh renderMesh;
    Canvas gridCanvas = null;

    public Vector2 offsetOrigin;

    // Use this for initialization
    void Start() {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(HexTerrain parent, float[,] heightMap, Vector2 origin) {
        terrain = parent;
        size = parent.chunkSize;
        offsetOrigin = origin;

        collisionMesh = GetComponent<TerrainCollisionMesh>();
        collisionMesh.Init();
        renderMesh = GetComponent<TerrainRenderMesh>();
        renderMesh.Init();

        transform.parent = parent.transform;
        transform.position = parent.transform.position;
        transform.localPosition = new Vector3(HexMetrics.innerRadius * 2 * origin.x, 0, HexMetrics.outerRadius * 1.5f * origin.y);

        if(showCoordinates) {
            InitCanvas();
        }

        cellStacks = new CellStack[size, size];

        for (int z = 0, i = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {

                int stackHeight = (int)(heightMap[x, z] * terrain.maxHeight);
                cellStacks[x, z] = CreateCellStack(x, z, stackHeight);
            }
        }

        GenerateMeshes();

        initialized = true;
    }

    void InitCanvas() {
        gridCanvas = new GameObject("Canvas").AddComponent<Canvas>();
        gridCanvas.renderMode = RenderMode.WorldSpace;
        gridCanvas.transform.parent = transform;
        gridCanvas.transform.localEulerAngles = new Vector3(90, 0, 0);
        gridCanvas.transform.position = transform.position;
        RectTransform rectTransform = gridCanvas.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 0);

        //Scale canvas to only cover the chunk
        rectTransform.sizeDelta = new Vector2(size * HexMetrics.innerRadius * 2, (size - 1) * HexMetrics.outerRadius * 1.5f);
    }

    CellStack CreateCellStack(int x, int z, int height) {

        CellStack cellStack = ScriptableObject.CreateInstance<CellStack>();
        cellStack.coordinates = HexCoordinates.FromOffsetCoordinates(x + (int)offsetOrigin.x, z + (int)offsetOrigin.y);

        cellStack.Push(CellType.Bedrock);

        int numWaterTiles = terrain.waterLevel - height;

        for (int i = 0; i < Math.Max(height, terrain.waterLevel); i++) {
            CellType newCell;

            if (numWaterTiles > 0 && i >= height) {
                newCell = CellType.Water;
            }
            else if (numWaterTiles <= 0 && i == height - 1) {
                newCell = CellType.Grass;
            }
            else {
                newCell = CellType.Dirt;
            }

            cellStack.Push(newCell);
        }

        if (showCoordinates && gridCanvas != null) {
            Vector3 position = cellStack.coordinates.ToChunkPosition();
            position += HexMetrics.heightVector * (cellStack.Count() + 1);

            Text label = Instantiate<Text>(terrain.cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D = new Vector3(position.x, position.z, -position.y);
            label.text = cellStack.coordinates.ToStringOnSeparateLines();
            //label.text = "" + cellStack.Count();
        }

        return cellStack;
    }

    public void GenerateMeshes() {
        collisionMesh.Generate();
        renderMesh.Generate();
    }

    public CellStack GetCellStackFromWorldCoords(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();
        return GetCellStackFromWorldOffset(offsetCoords);
    }

    public CellStack GetCellStackFromChunkCoords(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();
        return GetCellStackFromChunkOffset(offsetCoords);
    }

    public CellStack GetCellStackFromWorldOffset(Vector2 coords) {
        coords -= offsetOrigin;
        return GetCellStackFromChunkOffset(coords);
    }

    public CellStack GetCellStackFromChunkOffset(Vector2 coords) {
        CellStack stack = null;
        try {
            stack = cellStacks[(int)coords.x, (int)coords.y];
        }
        catch (IndexOutOfRangeException e) {
            Debug.LogWarning("WARNING: Tried to access CellStack that is out of bounds");
        }
        return stack;
    }

    public void AddCell(CellType cell, HexCoordinates coords) {
        CellStack stack = GetCellStackFromWorldCoords(coords);
        stack.Push(cell);
        GenerateMeshes();
    }

    public bool CanRemoveCell(HexCoordinates coordinates) {
        CellStack stack = GetCellStackFromWorldCoords(coordinates);
        return stack.CanPop();
    }

    public void RemoveCell(HexCoordinates coordinates) {
        CellStack stack = GetCellStackFromWorldCoords(coordinates);
        stack.Pop();
        GenerateMeshes();
    }
}
