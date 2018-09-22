using NewtonVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraform : MonoBehaviour {

    public Material ghostMaterial;

    HexCell cellType;

    HexGrid grid;

    // Use this for initialization
    void Start() {
        grid = GetComponentInParent<HexGrid>();
        cellType = ScriptableObject.CreateInstance<DirtCell>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void TouchCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        CellStack stack = grid.GetCellStack(coordinates);

        HexCell top = stack.Peek();

        //TODO draw ghost cell
        //Vector3 ghostPosition = top.transform.position;
        //ghostPosition += HexMetrics.heightVector;

        //ghostCell.transform.position = ghostPosition;
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
