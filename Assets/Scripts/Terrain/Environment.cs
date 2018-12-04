using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    HexTerrain terrain;
    public GameObject pedestal;
    public GameObject room;
    public GameObject floor;
    public GameObject player;

    public int pedestalHeight = 30;
    public int pedestalRimSize = 5;

    private float pedestalWidth;
    private float pedestalDepth;

    // Use this for initialization
    void Start () {
        ScalePedestal();
        ScaleTerrain();
    }

    private void ScalePedestal() {
        pedestalWidth = pedestal.GetComponent<MeshRenderer>().bounds.size.x;
        pedestalDepth = pedestal.GetComponent<MeshRenderer>().bounds.size.z;
        GameUtils.ScaleGameObjectToSize(pedestal, new Vector3(pedestalWidth, pedestalHeight, pedestalDepth));
    }

    // Scale the terrain so it fits on the pedestal
    private void ScaleTerrain() {
        terrain = GameObject.FindObjectOfType<HexTerrain>();

        float terrainWidth = terrain.GetWidth();
        float terrainDepth = terrain.GetDepth();

        Vector3 scale = terrain.transform.localScale;

        scale.z = (pedestalDepth - pedestalRimSize * 2) * scale.z / terrainDepth;
        scale.x = (pedestalWidth - pedestalRimSize * 2) * scale.x / terrainWidth;
        scale.y = (scale.x + scale.z) / 2;

        terrain.transform.localScale = scale;

        float terrainX = pedestal.transform.position.x - (terrain.GetWidth() / 2);
        float terrainZ = pedestal.transform.position.z - (terrain.GetDepth() / 2);
        terrain.transform.position = new Vector3(terrainX, pedestalHeight, terrainZ);

        
    }
}
