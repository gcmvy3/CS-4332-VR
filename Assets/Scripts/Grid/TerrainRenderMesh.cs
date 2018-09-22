using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(HexGrid))]
public class TerrainRenderMesh : MonoBehaviour {

    public Material missingMaterial;

    public Material bedrockTop;
    public Material dirtTop;
    public Material grassTop;

    public Material bedrockSide;
    public Material dirtSide;
    public Material grassSide;

    enum Materials { missingMaterial, bedrockTop, dirtTop, grassTop, bedrockSide, dirtSide, grassSide };

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

        renderObject = new GameObject();
        renderObject.name = "Render Mesh";
        renderObject.transform.SetParent(gameObject.transform);
        renderObject.transform.localPosition = new Vector3(0, 0, 0);

        Debug.Log("HERE");

        terrainRenderMesh = new Mesh();
        terrainRenderMesh.name = "TerrainRenderMesh";

        hexGrid = GetComponent<HexGrid>();
        meshFilter = renderObject.AddComponent<MeshFilter>();
        meshRenderer = renderObject.AddComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        meshFilter.sharedMesh = terrainRenderMesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        submeshes = new Dictionary<Material, List<int>>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void Generate() {
        terrainRenderMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        submeshes.Clear();

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
        terrainRenderMesh.subMeshCount = submeshes.Count;

        //Set material for each submesh
        meshRenderer.sharedMaterials = submeshes.Keys.ToArray();

        Debug.Log("Materials: " + meshRenderer.sharedMaterials[0].name);
        Debug.Log("Submeshes: " + submeshes[0);

        for(int i = 0; i < meshRenderer.materials.Count(); i++) {
            terrainRenderMesh.SetTriangles(submeshes[meshRenderer.materials[i]], i);
        }

        terrainRenderMesh.RecalculateNormals();
        terrainRenderMesh.RecalculateBounds();
    }

    void GenerateStackMesh(CellStack stack) {

        int stackHeight = stack.Count();
        HexCell topCell = stack.Peek();

        Material topMaterial = getTopMaterial(topCell);

        if(topMaterial == null) {
            Debug.Log("Material is NULL!");
        }

        List<int> topMaterialList = getSubmesh(topMaterial);

        Vector3 center = stack.coordinates.ToPosition();
        center += stackHeight * HexMetrics.heightVector;

        HexCoordinates[] neighbors = stack.coordinates.GetNeighbors();

        for (int i = 0; i < 6; i++) {
            //Generates the horizontal part of the terrain (top of the stack)
            topMaterialList.Add(triangleIndex);
            AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );

            //Generates the vertical part of the terrain (sides of the stack)
            CellStack neighbor = hexGrid.GetCellStack(neighbors[i]);

            //If we have a neighbor in this direction, check its height
            //If it is taller than us, ignore it (it will create the vertical wall)
            //If it is the same height as us, ignore it (we don't need a vertical wall)
            //If it is shorter than us, create a vertical wall down to its height
            if (neighbor != null) {
                int neighborHeight = neighbor.Count();

                float wallHeight = (stackHeight - neighborHeight) * HexMetrics.heightVector.y;

                if (wallHeight > 0) {
                    Vector3 wallHeightVector = new Vector3(0, wallHeight, 0);

                    AddTriangle(
                        center + HexMetrics.corners[i] - wallHeightVector,
                        center + HexMetrics.corners[i + 1],
                        center + HexMetrics.corners[i]

                    );

                    AddTriangle(
                        center + HexMetrics.corners[i] - wallHeightVector,
                        center + HexMetrics.corners[i + 1] - wallHeightVector,
                        center + HexMetrics.corners[i + 1]
                    );
                }
            }
            else {
                AddTriangle(
                    center + HexMetrics.corners[i] - HexMetrics.heightVector * stackHeight,
                    center + HexMetrics.corners[i + 1],
                    center + HexMetrics.corners[i]

                );

                AddTriangle(
                    center + HexMetrics.corners[i] - HexMetrics.heightVector * stackHeight,
                    center + HexMetrics.corners[i + 1] - HexMetrics.heightVector * stackHeight,
                    center + HexMetrics.corners[i + 1]
                );
            }
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {

        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangleIndex++;
    }

    private List<int> getSubmesh(Material m) {
        if(!submeshes.ContainsKey(m)) {
            List<int> newList = new List<int>();
            submeshes.Add(m, newList);
        }

        return submeshes[m];
    }

    private Material getTopMaterial(HexCell cell) {
        System.Type cellType = cell.GetType();
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
        System.Type cellType = cell.GetType();
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
