using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    HexTerrain terrain;
    GameObject pedestal;
    GameObject room;

    public int pedestalHeight = 5;
    public int pedestalRimSize = 10;

    // Use this for initialization
    void Start () {
        InitPedestal();
        InitRoom();

        // Make sure the terrain is at the same height as the player
        GameObject player = GameObject.Find("NVRPlayer");
        if (player)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y + pedestalHeight, transform.position.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitPedestal() {
        terrain = GameObject.FindObjectOfType<HexTerrain>();
        pedestal = GameObject.Instantiate(GameObject.Find("Cloneables/Pedestal"));

        float pedestalWidth = terrain.chunkSize * terrain.chunkColumns * (HexMetrics.innerRadius * 2) + HexMetrics.innerRadius + pedestalRimSize * 2;
        float pedestalDepth = terrain.chunkSize * terrain.chunkRows * HexMetrics.outerRadius * 1.5f + pedestalRimSize * 2;

        GameUtils.ScaleGameObjectToSize(pedestal, new Vector3(pedestalWidth, pedestalHeight, pedestalDepth));

        float pedestalX = terrain.transform.position.x + pedestalWidth / 2 - HexMetrics.innerRadius - pedestalRimSize;
        float pedestalZ = terrain.transform.position.z + pedestalDepth / 2 - HexMetrics.outerRadius - pedestalRimSize;
        pedestal.transform.position = new Vector3(pedestalX, terrain.transform.position.y - pedestalHeight, pedestalZ);
    }

    private void InitRoom()
    {
        room = GameObject.Instantiate(GameObject.Find("Cloneables/Room"));
        room.transform.localScale = pedestal.transform.localScale;
        room.transform.position = pedestal.transform.position;
    }
}
