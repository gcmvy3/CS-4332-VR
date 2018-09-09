using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

	public HexCoordinates coordinates;

    // Use this for initialization
    public virtual void Start () {
        scaleToCellSize();
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}

    public void scaleToCellSize() {
        float targetSize = HexMetrics.outerRadius * 2;
        float currentSize = GetComponent<MeshRenderer>().bounds.size.z;

        float targetHeight = HexMetrics.height;
        float currentHeight = GetComponent<MeshRenderer>().bounds.size.y;

        Vector3 scale = transform.localScale;

        scale.z = targetSize * scale.z / currentSize;
        scale.x = scale.z;
        scale.y = targetHeight * scale.y / currentHeight;

        transform.localScale = scale;
    }
}
