using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CielaSpike;

public class TerrainChunk : MonoBehaviour {

    
    public HexTerrain terrain;
    public int size;
    
    public bool showCoordinates = false;
    bool initialized = false;

    CellStack[,] cellStacks;
    TerrainObject[,] terrainObjects;

    public TerrainObject treePrefab;

    TerrainCollisionMesh collisionMesh;
    TerrainRenderMesh renderMesh;
    GameObject waterMesh = null;

    Canvas gridCanvas = null;

    public Vector2 offsetOrigin;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Init(HexTerrain parent, float[,] heightMap, float[,] treeMap, Vector2Int origin) {
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

        if (showCoordinates) {
            InitCanvas();
        }

        cellStacks = new CellStack[size, size];

        // First pass generates terrain from heightmap
        for (int z = 0, i = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {

                int stackHeight = (int)(heightMap[x, z] * terrain.maxHeight);
                cellStacks[x, z] = CreateCellStack(x, z, stackHeight);
            }
        }

        terrainObjects = new TerrainObject[size, size];
        bool[,] trees = new bool[size, size];

        // Second pass adds trees and sand
        for (int z = 0, i = 0; z < size; z++) {
            for (int x = 0; x < size; x++) {

                AddSand(cellStacks[x, z]);
                float treeChance = treeMap[x, z];
                if (treeChance > (1 - terrain.treeChance)) {
                    AddTree(cellStacks[x, z]);
                }
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
        HexCoordinates coords = HexCoordinates.FromOffsetCoordinates(x + (int)offsetOrigin.x, z + (int)offsetOrigin.y);
        Vector2Int indexWithinChunk = new Vector2Int(x % size, z % size);
        cellStack.Init(coords, indexWithinChunk);
        cellStack.Push(CellType.Bedrock);

        int numStoneTiles = (int)(height * terrain.stoneHeightPercentage);

        // Generate terrian based on stack height
        for (int i = 0; i < height; i++) {
            CellType newCell;

            if (height >= terrain.waterLevel - 1 && i == height - 1) {
                newCell = CellType.Grass;
            }
            else if(i < numStoneTiles) {
                newCell = CellType.Stone;
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
        }

        return cellStack;
    }

    // Converts the top cell to sand if certain criteria are met
    // This modifies terrain based on neighbor stacks
    // Thus it should only be called after all cell stacks are generated
    void AddSand(CellStack cellStack) {

        if (cellStack.Count() == terrain.waterLevel) {
            HexCoordinates[] neighbors = cellStack.coordinates.GetNeighbors();
            for (int i = 0; i < 6; i++) {
                CellStack neighbor = GetCellStackFromWorldCoords(neighbors[i]);

                // The neighbor cell stack might not be in this chunk
                if (neighbor == null) {
                    TerrainChunk neighborChunk = terrain.GetChunkFromWorldCoords(neighbors[i]);
                    if (neighborChunk != null) {
                        neighbor = neighborChunk.GetCellStackFromWorldCoords(neighbors[i]);
                    }
                }

                if (neighbor != null) {
                    if (neighbor.Count() < terrain.waterLevel) {
                        cellStack.Pop();
                        cellStack.Push(CellType.Sand);
                        break;
                    }
                }
            }
        }
    }

    // Adds a tree on top of stack if certain criteria are met
    void AddTree(CellStack cellStack) {

        if(cellStack.Count() > terrain.waterLevel && cellStack.Peek() == CellType.Grass) {
            Vector2Int offset = cellStack.indexWithinChunk;

            Vector3 treePos = transform.position + new Vector3(offset.x * HexMetrics.innerRadius * 2 + offset.y % 2 * HexMetrics.innerRadius,
                                                                transform.localPosition.y + HexMetrics.height * cellStack.Count(),
                                                                offset.y * HexMetrics.outerRadius * 1.5f);

            // Randomly rotate the tree so it looks less uniform
            float rotation = UnityEngine.Random.Range(0, 359);
            Vector3 treeRot = new Vector3(0, rotation, 0);

            TerrainObject tree = GameObject.Instantiate(treePrefab);
            tree.Init(cellStack.coordinates, offset);
            tree.transform.SetParent(transform);
            tree.transform.position = treePos;
            tree.transform.rotation = Quaternion.Euler(treeRot);
            terrainObjects[offset.x, offset.y] = tree;
        }
    }

    public void GenerateMeshes() {
        renderMesh.Generate();
        collisionMesh.Generate();

        if (waterMesh == null) {
            GenerateWaterMesh();
        }
    }

    void GenerateWaterMesh() {
        waterMesh = Instantiate(terrain.GetWaterMesh());
        waterMesh.transform.parent = transform;
        waterMesh.transform.position = transform.position;
        waterMesh.transform.position += (terrain.waterLevel - 1) * HexMetrics.heightVector;
        waterMesh.transform.position += HexMetrics.heightVector * terrain.shorelineWaterHeight;
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
        if(stack) {
            // Only trees can be placed on top of grass
            // So if something else is placed on top of grass, turn the grass into dirt
            if (cell != CellType.Trees && stack.Peek() == CellType.Grass) {
                stack.Pop();
                stack.Push(CellType.Dirt);
            }

            stack.Push(cell);
            GenerateMeshes();
        }
    }

    public bool CanRemoveCell(HexCoordinates coordinates) {
        CellStack stack = GetCellStackFromWorldCoords(coordinates);
        if(stack) {
            return stack.CanPop();
        }
        else {
            return false;
        }
    }

    public void RemoveCell(HexCoordinates coordinates) {
        CellStack stack = GetCellStackFromWorldCoords(coordinates);
        if(stack) {
            stack.Pop();
            GenerateMeshes();
        }
    }

    public void RemoveTerrainObject(HexCoordinates coordinates) {
        //TODO this could be more efficient
        CellStack stack = GetCellStackFromWorldCoords(coordinates);
        Vector2Int index = stack.indexWithinChunk;
        TerrainObject obj = terrainObjects[index.x, index.y];
        if(obj != null) {
            terrainObjects[index.x, index.y] = null;
            GameObject.Destroy(obj.gameObject);
        }
    }

    public void SetVisible(bool visible) {
        renderMesh.enabled = visible;
        collisionMesh.enabled = visible;
    }
}
