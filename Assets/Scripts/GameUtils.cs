using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour {

    public static float[,] GenerateNoiseMap(int rows, int columns, float scale) {
        // create an empty noise map
        float[,] noiseMap = new float[rows, columns];

        for (int zIndex = 0; zIndex < rows; zIndex++) {
            for (int xIndex = 0; xIndex < columns; xIndex++) {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = (float)(xIndex) / scale;
                float sampleZ = (float)(zIndex) / scale;

                // generate noise value using PerlinNoise
                float noise = Mathf.PerlinNoise(sampleX, sampleZ);

                noiseMap[zIndex, xIndex] = noise;
            }
        }

        return noiseMap;
    }

    public static float[,] Get2DArraySubset(float[,] original, Vector2Int start, Vector2Int end) {
        int startRow = start.y;
        int startCol = start.x;
        int endRow = end.y;
        int endCol = end.x;

        int numRows = endRow - startRow;
        int numCols = endCol - startCol;

        float[,] subset = new float[numCols, numRows];

        for (int z = startRow, b = 0; z < endRow; z++, b++) {
            for (int x = startCol, a = 0; x < endCol; x++, a++) {
                subset[a, b] = original[x, z];
            }
        }

        return subset;
    }

    public static void ScaleGameObjectToSize(GameObject obj, Vector3 targetSize) {

        Vector3 currentSize = obj.GetComponent<MeshRenderer>().bounds.size;

        Vector3 scale = obj.transform.localScale;

        scale.z = targetSize.z * scale.z / currentSize.z;
        scale.x = targetSize.x * scale.x / currentSize.x;
        scale.y = targetSize.y * scale.y / currentSize.y;

        obj.transform.localScale = scale;
    }
}