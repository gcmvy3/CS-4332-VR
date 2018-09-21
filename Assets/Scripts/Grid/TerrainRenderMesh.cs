using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(HexGrid))]
public class TerrainRenderMesh : MonoBehaviour {

    GameObject renderObject;
    HexGrid hexGrid;
    Mesh terrainRenderMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    Dictionary<Material, List<int>> submeshes;

    private int triangleIndex = 0;

    // Use this for initialization
    void Start() {

        renderObject = new GameObject();
        renderObject.name = "Render Mesh";
        renderObject.transform.SetParent(gameObject.transform);
        renderObject.transform.localPosition = new Vector3(0, 0, 0);

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

        for (int i = 0; i < rows * columns; i++) {
            Stack<HexCell> stack = hexGrid.GetCellStack(i);

            if (stack != null) {
                GenerateStackMesh(stack);
            }
        }

        terrainRenderMesh.vertices = vertices.ToArray();
        terrainRenderMesh.triangles = triangles.ToArray();

        //Create a submesh for each material
        terrainRenderMesh.subMeshCount = submeshes.Count;

        //Set material for each submesh
        meshRenderer.materials = submeshes.Keys.ToArray();

        for(int i = 0; i < meshRenderer.materials.Count(); i++) {
            terrainRenderMesh.SetTriangles(submeshes[meshRenderer.materials[i]], i);
        }

        terrainRenderMesh.RecalculateNormals();
        terrainRenderMesh.RecalculateBounds();
    }

    void GenerateStackMesh(Stack<HexCell> stack) {

        HexCell topCell = stack.Peek();

        Material topMaterial = topCell.topMaterial;

        Debug.Log("Top cell is type: " + topCell.GetType());
        Debug.Log("Top cell material: " + topCell.topMaterial);

        List<int> topMaterialList = getMaterialList(topMaterial);

        Vector3 center = topCell.transform.localPosition;
        center.y += HexMetrics.height / 2;

        int stackHeight = stack.Count;

        HexCoordinates[] neighbors = topCell.coordinates.GetNeighbors();

        for (int i = 0; i < 6; i++) {
            //Generates the horizontal part of the terrain (top of the stack)
            topMaterialList.Add(triangleIndex);
            AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );

            //Generates the vertical part of the terrain (sides of the stack)
            Stack<HexCell> neighbor = hexGrid.GetCellStack(neighbors[i]);

            //If we have a neighbor in this direction, check its height
            //If it is taller than us, ignore it (it will create the vertical wall)
            //If it is the same height as us, ignore it (we don't need a vertical wall)
            //If it is shorter than us, create a vertical wall down to its height
            if (neighbor != null) {
                int neighborHeight = neighbor.Count;

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

    private List<int> getMaterialList(Material m) {
        if(!submeshes.ContainsKey(m)) {
            List<int> newList = new List<int>();
            submeshes.Add(m, newList);
        }

        return submeshes[m];
    }
}
