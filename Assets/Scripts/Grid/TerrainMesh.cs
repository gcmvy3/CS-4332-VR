using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter))]
public class TerrainMesh : MonoBehaviour {

    Mesh terrainMesh;
    List<Vector3> vertices;
    List<int> triangles;

	// Use this for initialization
	void Start () {
        GetComponent<MeshFilter>().mesh = terrainMesh = new Mesh();
        terrainMesh.name = "Terrain Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Generate(int rows, int columns, Stack<HexCell>[] stacks) {
        terrainMesh.Clear();
        vertices.Clear();
        triangles.Clear();

        for (int i = 0; i < stacks.Length; i++) {
            GenerateMesh(stacks[i]);
        }

        terrainMesh.vertices = vertices.ToArray();
        terrainMesh.triangles = triangles.ToArray();
        terrainMesh.RecalculateNormals();
    }

    
    public void GenerateMesh(Stack<HexCell> stack) {
        HexCell topCell = stack.Peek();

        Vector3 center = topCell.transform.localPosition;
        center.y += HexMetrics.height / 2;

        int stackHeight = stack.Count;

        for(int i = 0; i < 6; i++) {
            //Generates the horizontal part of the terrain (top of the stack)
            AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );

            //Generates the vertical part of the terrain (sides of the stack)
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
