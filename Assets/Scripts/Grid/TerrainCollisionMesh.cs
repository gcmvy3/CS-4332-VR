using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(HexGrid))]
public class TerrainCollisionMesh : MonoBehaviour {

    GameObject collisionObject;
    HexGrid hexGrid;
    Mesh terrainMesh;
    List<Vector3> vertices;
    List<int> triangles;
    MeshCollider meshCollider;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    NavMeshSurface navMeshSurface;

	// Use this for initialization
	void Start () {

        collisionObject = new GameObject();
        collisionObject.name = "Collision Mesh";
        collisionObject.transform.SetParent(gameObject.transform);
        collisionObject.transform.localPosition = new Vector3(0, 0, 0);

        terrainMesh = new Mesh();
        terrainMesh.name = "TerrainCollisionMesh";

        hexGrid = GetComponent<HexGrid>();
        meshFilter = collisionObject.AddComponent<MeshFilter>();
        meshCollider = collisionObject.AddComponent<MeshCollider>();
        meshRenderer = collisionObject.AddComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        meshFilter.sharedMesh = terrainMesh;
        meshCollider.sharedMesh = terrainMesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();

        navMeshSurface = collisionObject.AddComponent<NavMeshSurface>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Generate() {
        terrainMesh.Clear();
        vertices.Clear();
        triangles.Clear();

        int rows = hexGrid.rows;
        int columns = hexGrid.columns;

        for (int i = 0; i < rows * columns; i++) {
            Stack<HexCell> stack = hexGrid.GetCellStack(i);

            if(stack != null) {
                GenerateStackMesh(stack);
            }
        }

        terrainMesh.vertices = vertices.ToArray();
        terrainMesh.triangles = triangles.ToArray();
        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateBounds();

        meshCollider.sharedMesh = terrainMesh;

        navMeshSurface.BuildNavMesh();
    }
    
    void GenerateStackMesh(Stack<HexCell> stack) {

        HexCell topCell = stack.Peek();

        Vector3 center = topCell.coordinates.ToOffsetCoordinates();
        center.y += HexMetrics.height / 2;

        int stackHeight = stack.Count;

        HexCoordinates[] neighbors = topCell.coordinates.GetNeighbors();

        for (int i = 0; i < 6; i++) {
            //Generates the horizontal part of the terrain (top of the stack)
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

                if(wallHeight > 0) {
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
    }
}
