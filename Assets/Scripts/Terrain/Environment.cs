using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    HexTerrain terrain;
    public GameObject pedestal;
    public GameObject room;
    public GameObject floor;

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
            transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitPedestal() {
        terrain = GameObject.FindObjectOfType<HexTerrain>();
        pedestal = GameObject.Instantiate(pedestal);

        float pedestalWidth = terrain.chunkSize * terrain.chunkColumns * (HexMetrics.innerRadius * 2) + HexMetrics.innerRadius + pedestalRimSize * 2;
        float pedestalDepth = terrain.chunkSize * terrain.chunkRows * HexMetrics.outerRadius * 1.5f + pedestalRimSize * 2;

        GameUtils.ScaleGameObjectToSize(pedestal, new Vector3(pedestalWidth, pedestalHeight, pedestalDepth));

        float pedestalX = terrain.transform.position.x + pedestalWidth / 2 - HexMetrics.innerRadius - pedestalRimSize;
        float pedestalZ = terrain.transform.position.z + pedestalDepth / 2 - HexMetrics.outerRadius - pedestalRimSize;
        pedestal.transform.position = new Vector3(pedestalX, terrain.transform.position.y - pedestalHeight, pedestalZ);
        pedestal.transform.parent = transform;
    }

    private void InitRoom()
    {
        room = GameObject.Instantiate(room);
        room.transform.localScale = pedestal.transform.localScale;
        room.transform.position = pedestal.transform.position;
        room.transform.parent = transform;

        floor = GameObject.Instantiate(floor);
        floor.transform.localScale = pedestal.transform.localScale;
        floor.transform.position = pedestal.transform.position;
        floor.transform.parent = transform;
    }
}
