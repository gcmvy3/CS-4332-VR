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

    CellStack[] cellStacks;

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

        cellStacks = new Stack<HexCell>[rows * columns];

        float[,] heightMap = Utils.GenerateNoiseMap(rows, columns, terrainScale);

        for (int z = 0, i = 0; z < rows; z++) {
            for (int x = 0; x < columns; x++) {

                int stackHeight = (int)(heightMap[x, z] * maxHeight);
                CreateCellStack(x, z, stackHeight, i++);
            }
        }

        GenerateMeshes();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateCellStack(int x, int z, int height, int index) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        Stack<HexCell> cellStack = new Stack<HexCell>();

        BedrockCell bedrockCell = new BedrockCell(); ;
        //bedrockCell.transform.SetParent(transform, false);
        //bedrockCell.transform.localPosition = position;
        bedrockCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cellStack.Push(bedrockCell);

        position.y += HexMetrics.height;

        for(int i = 0; i < height; i++) {
            HexCell newCell;

            if(i == height - 1) {
                newCell = new GrassCell();
            }
            else {
                newCell = new DirtCell();
            }
            
            //newCell.transform.SetParent(transform, false);
            //newCell.transform.localPosition = position;
            newCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cellStack.Push(newCell);

            position.y += HexMetrics.height;
        }

        if (showCoordinates) {
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D =
                  new Vector3(position.x, position.z, -cellStack.Count * HexMetrics.height);
            label.text = bedrockCell.coordinates.ToStringOnSeparateLines();
        }

        cellStacks[index] = cellStack;
    }

    public Stack<HexCell> GetCellStack(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();

        int index = (int)(offsetCoords.y * columns + offsetCoords.x);

        if (index >= 0 && index < cellStacks.Length) {
            return cellStacks[index];
        }
        else {
            return null;
        }
    }

    public Stack<HexCell> GetCellStack(int index) {
        if(index >= 0 && index < cellStacks.Length) {
            return cellStacks[index];
        }
        else {
            return null;
        }
    }

    public void AddCell(HexCell cell, HexCoordinates coords) {
        Stack<HexCell> stack = GetCellStack(coords);

        HexCell top = stack.Peek();

        /*
        Vector3 newPosition = top.transform.position;
        newPosition += HexMetrics.heightVector;
        cell.transform.position = newPosition;

        cell.transform.parent = GetComponentInParent<Transform>();
        */
        stack.Push(cell);

        GenerateMeshes();
    }

    public void GenerateMeshes() {
        collisionMesh.Generate();
        renderMesh.Generate();
    }

    public bool CanRemoveCell(HexCoordinates coordinates) {

        Stack<HexCell> stack = GetCellStack(coordinates);

        if(stack.Count > 0) {
            HexCell top = stack.Peek();
            if(!(top.GetType() == typeof(BedrockCell))) {
                return true;
            }
        }
        return false;
    }

    public void RemoveCell(HexCoordinates coordinates) {
        Stack<HexCell> stack = GetCellStack(coordinates);

        HexCell removedCell = stack.Pop();
        //Destroy(removedCell.gameObject);
        removedCell = null;

        GenerateMeshes();
    }
}
