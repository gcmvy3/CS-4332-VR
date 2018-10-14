using NewtonVR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraform : MonoBehaviour {

    public Material ghostMaterial;
    GameObject ghostCellPrefab;
    CellType cellType;

    HexTerrain terrain;

    // Use this for initialization
    void Start() {
        terrain = gameObject.GetComponent<HexTerrain>();
        cellType = CellType.Dirt;
        initGhostCell();
    }

    // Update is called once per frame
    void Update() {

    }

    public void TouchCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);

        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);

        try {
            CellStack stack = chunk.GetCellStackFromWorldCoords(coordinates);

            Vector3 ghostPosition = stack.coordinates.ToWorldPosition(chunk) + HexMetrics.heightVector * stack.Count();
            ghostPosition += HexMetrics.heightVector / 2;
            ghostCellPrefab.transform.localPosition = ghostPosition;
        }
        catch (NullReferenceException e) {
            Debug.LogWarning("Trying to access null cellstack");
        }
    }

    public void PlaceCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);
        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);
        chunk.AddCell(cellType, coordinates);
    }

    public bool CanRemoveCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);
        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);
        return chunk.CanRemoveCell(coordinates);
    }

    public void RemoveCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);
        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);
        chunk.RemoveCell(coordinates);
    }

    private void initGhostCell() {
        ghostCellPrefab = GameObject.Instantiate(GameObject.Find("Cloneables/HexCellPrefab"));

        //Scale the cell so it is the correct size
        Vector3 targetSize = new Vector3(HexMetrics.innerRadius * 2, HexMetrics.height, HexMetrics.outerRadius * 2);
        GameUtils.ScaleGameObjectToSize(ghostCellPrefab, targetSize);

        //Set the initial position of the ghost cell
        ghostCellPrefab.transform.position = terrain.transform.position;

        //Apply the ghost material to the ghost cell
        MeshRenderer ghostRenderer = ghostCellPrefab.GetComponent<MeshRenderer>();
        Material[] ghostMaterials = new Material[ghostRenderer.materials.Length];
        for (int i = 0; i < ghostMaterials.Length; i++) {
            ghostMaterials[i] = ghostMaterial;
        }
        ghostRenderer.materials = ghostMaterials;
    }


}
