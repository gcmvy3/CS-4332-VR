using NewtonVR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraformController : MonoBehaviour {

    public bool enabled = false;

    public Material ghostMaterial;
    public GameObject ghostCellPrefab;

    public HexTerrain terrain;

    CellType cellType;

    Vector3 pointerOrigin;
    Vector3 pointerTarget;

    public Color PointerColor;
    public float PointerWidth = 0.02f;

    private LineRenderer Pointer;

    private bool clicked = false;

    // Use this for initialization
    void Start() {
        InitGhostCell();
        InitPointer();

        if(terrain == null) {
            terrain = FindObjectOfType<HexTerrain>();
        }
    }

    // Update is called once per frame
    void Update() {
        if(enabled) {
            Pointer.enabled = true;
            UpdatePointer();
        }
        else if (Pointer.enabled) {
            Pointer.enabled = false;
        }
    }

    private void InitPointer() {
        Pointer = this.GetComponent<LineRenderer>();
        if (Pointer == null) {
            Pointer = this.gameObject.AddComponent<LineRenderer>();
        }
        if (Pointer.sharedMaterial == null) {
            Pointer.material = new Material(Shader.Find("Unlit/Color"));
            Pointer.material.SetColor("_Color", PointerColor);
            NVRHelpers.LineRendererSetColor(Pointer, PointerColor, PointerColor);
        }

        Pointer.useWorldSpace = true;
    }

    private void UpdatePointer() {
        Pointer.material.SetColor("_Color", PointerColor);
        NVRHelpers.LineRendererSetColor(Pointer, PointerColor, PointerColor);
        NVRHelpers.LineRendererSetWidth(Pointer, PointerWidth, PointerWidth);

        pointerOrigin = transform.position;
        //TODO fix pointer angle
        pointerTarget = transform.forward;

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(pointerOrigin, pointerTarget, out hitInfo, 1000);
        Vector3 endPoint;

        if (hit == true) {
            //1.01 extends the ray slightly to avoid landing on a hex boundary
            endPoint = hitInfo.point * 1.01f;
            this.TouchCell(endPoint);

            //If button is clicked, add or remove cell
            if(clicked) {
                clicked = false;
                if(cellType == CellType.Missing && this.CanRemoveCell(endPoint)) {
                    this.RemoveCell(endPoint);
                }
                else {
                    PlaceCell(endPoint);
                }
            }
        }
        else {
            endPoint = pointerOrigin + (pointerTarget * 1000f);
        }

        Pointer.SetPositions(new Vector3[] { pointerOrigin, endPoint });
    }

    public void Click() {
        clicked = true;
    }

    private void TouchCell(Vector3 position) {
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

    private void PlaceCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);
        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);
        if(chunk) {
            chunk.RemoveTerrainObject(coordinates);
            chunk.AddCell(cellType, coordinates);
        }
    }

    public bool CanRemoveCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);
        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);
        if(chunk) {
            return chunk.CanRemoveCell(coordinates);
        }
        else {
            return false;
        }
    }

    public void RemoveCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromGlobalPosition(position);
        TerrainChunk chunk = terrain.GetChunkFromWorldCoords(coordinates);
        if(chunk) {
            chunk.RemoveTerrainObject(coordinates);
            chunk.RemoveCell(coordinates);
        }
    }

    public void SetCellType(CellType type) {
        cellType = type;
    }

    private void InitGhostCell() {
        ghostCellPrefab = GameObject.Instantiate(ghostCellPrefab);
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
