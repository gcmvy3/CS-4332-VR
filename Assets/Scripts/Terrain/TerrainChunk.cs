﻿using System;
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
    Canvas gridCanvas;

    public Vector2 offsetOrigin;

    // Use this for initialization
    void Start() {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(HexTerrain parent, float[,] heightMap, Vector2 origin) {
        collisionMesh = GetComponent<TerrainCollisionMesh>();
        collisionMesh.Init();
        renderMesh = GetComponent<TerrainRenderMesh>();
        renderMesh.Init();

        transform.parent = parent.transform;
        transform.position = parent.transform.localPosition;
        transform.localPosition = new Vector3(HexMetrics.innerRadius * 2 * origin.x, 0, HexMetrics.outerRadius * 1.5f * origin.y);
        //transform.localPosition = new Vector3(0, 0, 0);

        gridCanvas = GetComponentInChildren<Canvas>();

        if(gridCanvas == null) {
            gridCanvas.transform.position = transform.position;
            gridCanvas.transform.localPosition = transform.localPosition;
        }

        terrain = parent;
        size = parent.chunkSize;
        offsetOrigin = origin;
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

    CellStack CreateCellStack(int x, int z, int height) {

        CellStack cellStack = ScriptableObject.CreateInstance<CellStack>();
        cellStack.coordinates = HexCoordinates.FromOffsetCoordinates(x + (int)offsetOrigin.x, z + (int)offsetOrigin.y);

        BedrockCell bedrockCell = ScriptableObject.CreateInstance<BedrockCell>();
        cellStack.Push(bedrockCell);

        int numWaterTiles = terrain.waterLevel - height;

        for (int i = 0; i < Math.Max(height, terrain.waterLevel); i++) {
            HexCell newCell;

            if (numWaterTiles > 0 && i >= height) {
                newCell = ScriptableObject.CreateInstance<WaterCell>();
            }
            else if (numWaterTiles <= 0 && i == height - 1) {
                newCell = ScriptableObject.CreateInstance<GrassCell>();
            }
            else {
                newCell = ScriptableObject.CreateInstance<DirtCell>();
            }

            cellStack.Push(newCell);
        }

        if (showCoordinates && gridCanvas != null) {
            Vector3 position = cellStack.coordinates.ToLocalPosition();
            position += HexMetrics.heightVector * (cellStack.Count() + 1);

            Text label = Instantiate<Text>(terrain.cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D = new Vector3(position.x, position.z, -position.y);
            label.text = cellStack.coordinates.ToStringOnSeparateLines();
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

    public void AddCell(HexCell cell, HexCoordinates coords) {
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
