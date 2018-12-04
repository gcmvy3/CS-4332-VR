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

    CellType cellType = CellType.Missing;

    Vector3 pointerOrigin;
    Vector3 pointerTarget;

    public Color PointerColor;
    public float PointerWidth = 0.02f;

    private LineRenderer Pointer;

    private bool clicked = false;

    public GameObject pointerTracker = null;

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
        //TODO is this necessary here?
        Pointer.material.SetColor("_Color", PointerColor);
        NVRHelpers.LineRendererSetColor(Pointer, PointerColor, PointerColor);
        NVRHelpers.LineRendererSetWidth(Pointer, PointerWidth, PointerWidth);

        pointerOrigin = transform.position;
        //TODO fix pointer angle
        pointerTarget = transform.forward;

        ghostCellPrefab.SetActive(false);

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(pointerOrigin, pointerTarget, out hitInfo, 1000);
        Vector3 endPoint;

        if (hit == true) {
            //1.01 extends the ray slightly to avoid landing on a hex boundary
            endPoint = hitInfo.point * 1.01f;

            if(pointerTracker != null) {
                pointerTracker.transform.position = endPoint;
            }

            //Check if we hit a terrain chunk
            GameObject hitObjectParent = hitInfo.transform.parent.gameObject;
            TerrainChunk hitChunk = hitObjectParent.GetComponent<TerrainChunk>();

            if(hitChunk) {
                HexCoordinates worldCoordinates = HexCoordinates.WorldCoordsFromGlobalPosition(terrain, endPoint);

                Vector2 worldOffset = worldCoordinates.ToOffsetCoordinates();

                this.TouchCell(hitChunk, worldCoordinates);

                //If button is clicked, edit chunk
                if (clicked) {
                    clicked = false;
                    if (cellType == CellType.Missing && hitChunk.CanRemoveCell(worldCoordinates)) {
                        hitChunk.RemoveTerrainObject(worldCoordinates);
                        hitChunk.RemoveCell(worldCoordinates);
                    }
                    else {
                        hitChunk.AddCell(cellType, worldCoordinates);
                    }

                    //Send pulse to controller if terraform was successful
                    InputController inputController = FindObjectOfType<InputController>();
                    if(inputController) {
                        inputController.TriggerPrimaryHandPulse();
                    }
                }
            }
        }
        else {
            //If we missed, extend the pointer out in a straight line
            endPoint = pointerOrigin + (pointerTarget * 1000f);
        }

        Pointer.SetPositions(new Vector3[] { pointerOrigin, endPoint });
    }

    public void Click() {
        clicked = true;
    }

    private void TouchCell(TerrainChunk chunk, HexCoordinates coords) {
        try {
            CellStack stack = chunk.GetCellStackFromWorldCoords(coords);

            ghostCellPrefab.SetActive(true);

            Vector3 ghostPosition = stack.coordinates.ToWorldPosition(terrain) + HexMetrics.ScaledHeightVector(terrain) * stack.Count();
            ghostCellPrefab.transform.localPosition = ghostPosition;
        }
        catch (NullReferenceException e) {
            Debug.LogWarning("Trying to access null cellstack");
        }
    }

    public void SetCellType(CellType type) {
        cellType = type;
    }

    private void InitGhostCell() {
        ghostCellPrefab = GameObject.Instantiate(ghostCellPrefab);
        //Scale the cell so it is the correct size
        Vector3 targetSize = new Vector3(HexMetrics.ScaledInnerRadius(terrain) * 2, HexMetrics.ScaledHeight(terrain), HexMetrics.ScaledOuterRadius(terrain) * 2);
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
