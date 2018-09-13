using NewtonVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraform : MonoBehaviour {

    public Material ghostMaterial;

    HexCell cellType;
    HexCell ghostCell;

    HexGrid grid;

    // Use this for initialization
    void Start() {

        grid = GetComponentInParent<HexGrid>();
        SetCellType(grid.dirtCellPrefab);
    }

    // Update is called once per frame
    void Update() {

    }

    public void TouchCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        Stack<HexCell> stack = grid.GetCellStack(coordinates);

        HexCell top = stack.Peek();

        Vector3 ghostPosition = top.transform.position;
        ghostPosition += HexMetrics.heightVector;

        ghostCell.transform.position = ghostPosition;
    }

    public void SetCellType(HexCell cell) {
        cellType = Instantiate<HexCell>(cell);
        ghostCell = Instantiate<HexCell>(cell);

        Renderer renderer = ghostCell.GetComponentInChildren<Renderer>();
        if(renderer != null) {

            Material[] ghostMaterials = new Material[renderer.materials.Length];

            for (int i = 0; i < ghostMaterials.Length; i++) {
                ghostMaterials[i] = ghostMaterial;
            }

            renderer.materials = ghostMaterials;
        }

        ghostCell.GetComponent<Renderer>().material = ghostMaterial;
    }

    public void PlaceCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        grid.AddCell(Instantiate<HexCell>(cellType), coordinates);
    }

    public bool CanRemoveCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        return grid.CanRemoveCell(coordinates);
    }

    public void RemoveCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        grid.RemoveCell(coordinates);
    }
}
