﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {

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

    public static float[,] Get2DArraySubset(float[,] original, Vector2 start, Vector2 end) {
        int startRow = (int)start.y;
        int startCol = (int)start.x;
        int endRow = (int)end.y;
        int endCol = (int)end.x;

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
}