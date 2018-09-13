using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

    public int columns = 12;
    public int rows = 24;
    public bool showCoordinates = true;

    public HexCell hexCellPrefab;
    public BedrockCell bedrockCellPrefab;
    public DirtCell dirtCellPrefab;

    public Text cellLabelPrefab;

    Stack<HexCell>[] cellStacks;

    Canvas gridCanvas;

    TerrainMesh terrainMesh;

    // Use this for initialization
    void Start () {
        gridCanvas = GetComponentInChildren<Canvas>();
        terrainMesh = GetComponentInChildren<TerrainMesh>();

        cellStacks = new Stack<HexCell>[rows * columns];

        for (int z = 0, i = 0; z < rows; z++)
        {
            for (int x = 0; x < columns; x++)
            {
                CreateCellStack(x, z, i++);
            }
        }

        GenerateTerrainMesh();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateCellStack(int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        Stack<HexCell> cellStack = new Stack<HexCell>();

        BedrockCell bedrockCell = Instantiate<BedrockCell>(bedrockCellPrefab);
        bedrockCell.transform.SetParent(transform, false);
        bedrockCell.transform.localPosition = position;
        bedrockCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cellStack.Push(bedrockCell);

        position.y += HexMetrics.height;

        if(i % 2 == 0) {
            DirtCell dirtCell = Instantiate<DirtCell>(dirtCellPrefab);
            dirtCell.transform.SetParent(transform, false);
            dirtCell.transform.localPosition = position;
            dirtCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cellStack.Push(dirtCell);

            position.y += HexMetrics.height;
        }

        if (showCoordinates) {
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition3D =
                  new Vector3(position.x, position.z, -cellStack.Count * HexMetrics.height);
            label.text = bedrockCell.coordinates.ToStringOnSeparateLines();
        }

        cellStacks[i] = cellStack;
    }

    public Stack<HexCell> GetCellStack(HexCoordinates coords) {
        Vector2 offsetCoords = coords.ToOffsetCoordinates();

        int index = (int)(offsetCoords.y * columns + offsetCoords.x);

        return cellStacks[index];
    }

    public void AddCell(HexCell cell, HexCoordinates coords) {
        Stack<HexCell> stack = GetCellStack(coords);

        HexCell top = stack.Peek();

        Vector3 newPosition = top.transform.position;
        newPosition += HexMetrics.heightVector;
        cell.transform.position = newPosition;

        cell.transform.parent = GetComponentInParent<Transform>();

        stack.Push(cell);

        GenerateTerrainMesh();
    }

    public void GenerateTerrainMesh() {
        terrainMesh.Generate(rows, columns, cellStacks);
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
        Destroy(removedCell.gameObject);
        removedCell = null;

        GenerateTerrainMesh();
    }
}
