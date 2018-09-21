using System.Collections;
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
}
