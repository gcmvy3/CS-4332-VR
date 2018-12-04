using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics {

    public const float outerRadius = 1.6f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public const float height = 0.8f;

    public static Vector3 heightVector = new Vector3(0, height, 0);

    public static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static float ScaledInnerRadius(HexTerrain terrain) {
        return innerRadius * terrain.transform.localScale.x;
    }

    public static float ScaledOuterRadius(HexTerrain terrain) {
        return outerRadius * terrain.transform.localScale.z;
    }

    public static float ScaledHeight(HexTerrain terrain) {
        return height * terrain.transform.localScale.y;
    }

    public static Vector3 ScaledHeightVector(HexTerrain terrain) {
        return new Vector3(0, ScaledHeight(terrain), 0);
    }
}
