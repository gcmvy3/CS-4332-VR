using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class InputTest : MonoBehaviour {

    NVRPlayer player;
    NVRHand rightHand;
    NVRHand leftHand;

	// Use this for initialization
	void Start () {
        player = GameObject.FindObjectOfType<NVRPlayer>();
        rightHand = player.RightHand;
        leftHand = player.LeftHand;
    }
	
	// Update is called once per frame
	void Update () {

        Debug.Log("Updating");

		if(rightHand.HoldButtonDown) {
            Debug.Log("Right hand hold");
        }
	}
}
