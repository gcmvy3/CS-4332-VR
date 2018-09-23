using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

[RequireComponent(typeof(HexGrid))]
public class TerrainRenderMesh : MonoBehaviour {

    public Material missingMaterial;

    public Material bedrockTop;
    public Material dirtTop;
    public Material grassTop;

    public Material bedrockSide;
    public Material dirtSide;
    public Material grassSide;

    Material[] materials;

    GameObject renderObject;
    HexGrid hexGrid;
    Mesh terrainRenderMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    List<int>[] submeshes;

    private int triangleIndex = 0;

    // Use this for initialization
    void Awake() {

        materials = new[] {missingMaterial, bedrockTop, dirtTop, grassTop, bedrockSide, dirtSide, grassSide};

        renderObject = new GameObject();
        renderObject.name = "Render Mesh";
        renderObject.transform.SetParent(gameObject.transform);
        renderObject.transform.localPosition = new Vector3(0, 0, 0);

        terrainRenderMesh = new Mesh();
        terrainRenderMesh.name = "TerrainRenderMesh";
        terrainRenderMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        hexGrid = GetComponent<HexGrid>();
        meshFilter = renderObject.AddComponent<MeshFilter>();
        meshRenderer = renderObject.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = terrainRenderMesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        submeshes = new List<int>[materials.Count()];
    }

    // Update is called once per frame
    void Update() {

    }

    public void Generate() {
        terrainRenderMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        submeshes = new List<int>[materials.Count()];

        triangleIndex = 0;
        int rows = hexGrid.rows;
        int columns = hexGrid.columns;

        for (int z = 0, i = 0; z < rows; z++) {
            for (int x = 0; x < columns; x++) {
                CellStack stack = hexGrid.GetCellStack(new Vector2(x, z));

                if (stack != null) {
                    GenerateStackMesh(stack);
                }
            }
        }

        terrainRenderMesh.vertices = vertices.ToArray();
        terrainRenderMesh.triangles = triangles.ToArray();

        //Create a submesh for each material
        terrainRenderMesh.subMeshCount = submeshes.Count();

        //Set material for each submesh
        meshRenderer.materials = materials;

        for(int i = 0; i < submeshes.Count(); i++) {
            if(submeshes[i] != null) {
                terrainRenderMesh.SetTriangles(submeshes[i], i);
            }
        }

        terrainRenderMesh.RecalculateNormals();
        terrainRenderMesh.RecalculateBounds();
    }

    void GenerateStackMesh(CellStack stack) {

        int stackHeight = stack.Count();
        Vector3 center = stack.coordinates.ToPosition();
        center += stackHeight * HexMetrics.heightVector;

        HexCell topCell = stack.Peek();
        Material topMaterial = getTopMaterial(topCell);
        List<int> topMaterialSubmesh = getSubmesh(topMaterial);

        //Generates the horizontal part of the terrain (top of the stack)
        for (int i = 0; i < 6; i++) {
            AddTriangleToMesh(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );
            AddTriangleToSubmesh(
                topMaterialSubmesh,
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );
        }

        HexCoordinates[] neighbors = stack.coordinates.GetNeighbors();

        //Generates the vertical part of the terrain (sides of the stack)
        for (int i = 0; i < 6; i++) { 
            CellStack neighbor = hexGrid.GetCellStack(neighbors[i]);

            //If we have a neighbor in this direction, check its height
            //If it is taller than us, ignore it (it will create the vertical wall)
            //If it is the same height as us, ignore it (we don't need a vertical wall)
            //If it is shorter than us, create a vertical wall down to its height

            center = stack.coordinates.ToPosition();
            center += stackHeight * HexMetrics.heightVector;

            int wallHeight = stackHeight;

            if (neighbor != null) {
                wallHeight -= neighbor.Count();
            }
            
            for(int j = 0; j < wallHeight; j++) {

                HexCell currentCell = stack.PeekAt(stack.Count() - (j + 1));
                Material sideMaterial = getSideMaterial(currentCell);
                List<int> sideMaterialSubmesh = getSubmesh(sideMaterial);

                AddTriangleToMesh(
                    center + HexMetrics.corners[i] - HexMetrics.heightVector,
                    center + HexMetrics.corners[i + 1],
                    center + HexMetrics.corners[i]

                );
                AddTriangleToSubmesh(
                    sideMaterialSubmesh,
                    center + HexMetrics.corners[i] - HexMetrics.heightVector,
                    center + HexMetrics.corners[i + 1],
                    center + HexMetrics.corners[i]

                );
                AddTriangleToMesh(
                    center + HexMetrics.corners[i] - HexMetrics.heightVector,
                    center + HexMetrics.corners[i + 1] - HexMetrics.heightVector,
                    center + HexMetrics.corners[i + 1]
                );
                AddTriangleToSubmesh(
                    sideMaterialSubmesh,
                    center + HexMetrics.corners[i] - HexMetrics.heightVector,
                    center + HexMetrics.corners[i + 1] - HexMetrics.heightVector,
                    center + HexMetrics.corners[i + 1]
                );

                center -= HexMetrics.heightVector;
            }
        }
    }

    void AddTriangleToMesh(Vector3 v1, Vector3 v2, Vector3 v3) {

        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleToSubmesh(List<int> submesh, Vector3 v1, Vector3 v2, Vector3 v3) {
        submesh.Add(triangleIndex);
        submesh.Add(triangleIndex + 1);
        submesh.Add(triangleIndex + 2);
        triangleIndex += 3;
    }

    private List<int> getSubmesh(Material m) {
        int materialIndex = Array.IndexOf(materials, m);
        List<int> submesh = submeshes[materialIndex];
        if(submesh == null) {
            submesh = submeshes[materialIndex] = new List<int>();
        }
        return submesh;
    }

    private Material getTopMaterial(HexCell cell) {
        Type cellType = cell.GetType();

        if(cellType.Name == "BedrockCell") {
            return bedrockTop;
        }
        else if(cellType.Name == "DirtCell") {
            return dirtTop;
        }
        else if(cellType.Name == "GrassCell") {
            return grassTop;
        }
        else {
            return missingMaterial;
        }
    }

    private Material getSideMaterial(HexCell cell) {
        Type cellType = cell.GetType();
        if (cellType.Name == "BedrockCell") {
            return bedrockSide;
        }
        else if (cellType.Name == "DirtCell") {
            return dirtSide;
        }
        else if (cellType.Name == "GrassCell") {
            return grassSide;
        }
        else {
            return missingMaterial;
        }
    }
}
