using NewtonVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraform : MonoBehaviour {

    public GameObject ghostCellPrefab;
    public Material ghostMaterial;

    HexCell cellType;

    HexGrid grid;

    // Use this for initialization
    void Start() {
        grid = GetComponentInParent<HexGrid>();
        cellType = ScriptableObject.CreateInstance<DirtCell>();
        initGhostCell();
    }

    // Update is called once per frame
    void Update() {

    }

    public void TouchCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);

        CellStack stack = grid.GetCellStack(coordinates);

        Vector3 ghostPosition = stack.coordinates.ToGlobalPosition(grid) + HexMetrics.heightVector * stack.Count();
        ghostPosition += HexMetrics.heightVector / 2;
        ghostCellPrefab.transform.localPosition = ghostPosition;
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

    private void initGhostCell() {
        ghostCellPrefab = GameObject.Instantiate(ghostCellPrefab);

        //Scale the cell so it is the correct size
        float targetSize = HexMetrics.outerRadius * 2;
        float currentSize = ghostCellPrefab.GetComponent<MeshRenderer>().bounds.size.z;

        float targetHeight = HexMetrics.height;
        float currentHeight = ghostCellPrefab.GetComponent<MeshRenderer>().bounds.size.y;

        Vector3 scale = ghostCellPrefab.transform.localScale;

        scale.z = targetSize * scale.z / currentSize;
        scale.x = scale.z;
        scale.y = targetHeight * scale.y / currentHeight;

        ghostCellPrefab.transform.localScale = scale;

        //Set the initial position of the ghost cell
        ghostCellPrefab.transform.position = grid.transform.position;

        //Apply the ghost material to the ghost cell
        MeshRenderer ghostRenderer = ghostCellPrefab.GetComponent<MeshRenderer>();
        Material[] ghostMaterials = new Material[ghostRenderer.materials.Length];
        for (int i = 0; i < ghostMaterials.Length; i++) {
            ghostMaterials[i] = ghostMaterial;
        }
        ghostRenderer.materials = ghostMaterials;
    }


}
