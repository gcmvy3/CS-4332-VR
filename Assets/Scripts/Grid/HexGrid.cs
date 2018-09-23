using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TerrainCollisionMesh), typeof(TerrainRenderMesh))]
public class HexGrid : MonoBehaviour {

    public static float terrainScale = 5.0f;
    public int columns = 24;
    public int rows = 48;
    public int maxHeight = 6;
    public bool showCoordinates = true;

    public Text cellLabelPrefab;

    CellStack[,] cellStacks;

    Canvas gridCanvas;

    TerrainCollisionMesh collisionMesh;
    TerrainRenderMesh renderMesh;

    // Use this for initialization
    void Start () {
        gridCanvas = GetComponentInChildren<Canvas>();
        collisionMesh = GetComponent<TerrainCollisionMesh>();
        renderMesh = GetComponent<TerrainRenderMesh>();
        GenerateProceduralMap();
    }

    public void GenerateProceduralMap() {

        cellStacks = new CellStack[columns, rows];

        float[,] heightMap = Utils.GenerateNoiseMap(rows, columns, terrainScale);

        for (int z = 0, i = 0; z < rows; z++) {
            for (int x = 0; x < columns; x++) {

                int stackHeight = (int)(heightMap[x, z] * maxHeight);
                cellStacks[x, z] = CreateCellStack(x, z, stackHeight, i++);
            }
        }

        GenerateMeshes();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    CellStack CreateCellStack(int x, int z, int height, int index) {

        CellStack cellStack = new GameObject("CellStack" + index).AddComponent<CellStack>();
        cellStack.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cellStack.transform.position = transform.position;
        cellStack.transform.localPosition = cellStack.coordinates.ToLocalPosition();

        BedrockCell bedrockCell = ScriptableObject.CreateInstance<BedrockCell>();
        cellStack.Push(bedrockCell);

        for(int i = 0; i < height; i++) {
            HexCell newCell;

            if(i == height - 1) {
                newCell = ScriptableObject.CreateInstance<GrassCell>();
            }
            else {
                newCell = ScriptableObject.CreateInstance<DirtCell>();
            }

            cellStack.Push(newCell);
        }

        if (showCoordinates) {

            Vector3 position = cellStack.coordinates.ToLocalPosition();
            position += HexMetrics.heightVector * cellStack.Count();

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D =
                  new Vector3(position.x, position.z, -position.y);
            label.text = cellStack.coordinates.ToStringOnSeparateLines();
        }

        return cellStack;
    }

    public CellStack GetCellStack(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();

        CellStack stack = null;
        try {
            stack = cellStacks[(int)offsetCoords.x, (int)offsetCoords.y];
        }
        catch(System.IndexOutOfRangeException e) {
            Debug.Log("WARNING: Tried to access CellStack that is out of bounds");
        }
        return stack;
    }

    public CellStack GetCellStack(Vector2 coords) {
        return cellStacks[(int)coords.x, (int)coords.y];
    }

    public void AddCell(HexCell cell, HexCoordinates coords) {
        CellStack stack = GetCellStack(coords);
        stack.Push(cell);
        GenerateMeshes();
    }

    public void GenerateMeshes() {
        collisionMesh.Generate();
        renderMesh.Generate();
    }

    public bool CanRemoveCell(HexCoordinates coordinates) {
        CellStack stack = GetCellStack(coordinates);
        return stack.CanPop();
    }

    public void RemoveCell(HexCoordinates coordinates) {
        CellStack stack = GetCellStack(coordinates);
        stack.Pop();
        GenerateMeshes();
    }
}
