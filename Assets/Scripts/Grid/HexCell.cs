using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexCell : ScriptableObject {

    public CellStack cellStack;

    public Material topMaterial;
    public Material sideMaterial;

    public int elevation = -1;
    
    // Use this for initialization
    public virtual void Start () {
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
	}
}
