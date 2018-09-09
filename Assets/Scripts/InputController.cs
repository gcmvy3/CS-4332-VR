using NewtonVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    private NVRPlayer player;
    private NVRHand secondaryHand;
    private NVRHand primaryHand;

    public Color PointerColor;
    public float PointerWidth = 0.02f;

    private LineRenderer Pointer;

    private Vector3 prevHandPosition = new Vector3();

    // Use this for initialization
    void Start () {
        player = GetComponent<NVRPlayer>();
        primaryHand = player.RightHand;
        secondaryHand = player.LeftHand;

        initPointer();
    }
	
	// Update is called once per frame
	void Update () {

        //Allow "pulling" movement by gripping with secondary hand
        if(secondaryHand.HoldButtonDown) {
            prevHandPosition = secondaryHand.CurrentPosition;
        }
        else if(secondaryHand.HoldButtonPressed) {
            Vector3 delta = secondaryHand.transform.position - prevHandPosition;
            player.transform.Translate(-delta);
        }

        Pointer.enabled = true; //Hand.Inputs[NVRButtons.Trigger].IsPressed);

        if (Pointer.enabled == true) {
            Pointer.material.SetColor("_Color", PointerColor);
            NVRHelpers.LineRendererSetColor(Pointer, PointerColor, PointerColor);
            NVRHelpers.LineRendererSetWidth(Pointer, PointerWidth, PointerWidth);

            Vector3 pointerTarget;

            //TODO fix pointer angle
            //primaryHand.transform.RotateAround(primaryHand.transform.position, primaryHand.transform.forward, 45f);
            pointerTarget = primaryHand.transform.forward;
            //primaryHand.transform.RotateAround(primaryHand.transform.position, primaryHand.transform.forward, -45f);

            RaycastHit hitInfo;
            bool hit = Physics.Raycast(primaryHand.transform.position, pointerTarget, out hitInfo, 1000);
            Vector3 endPoint;

            if (hit == true) {
                endPoint = hitInfo.point;
            }
            else {
                endPoint = primaryHand.transform.position + (pointerTarget * 1000f);
            }

            Pointer.SetPositions(new Vector3[] { primaryHand.transform.position, endPoint });
        }
    }

    private void initPointer() {
        Pointer = this.GetComponent<LineRenderer>();
        if (Pointer == null) {
            Pointer = this.gameObject.AddComponent<LineRenderer>();
        }
        if (Pointer.sharedMaterial == null) {
            Pointer.material = new Material(Shader.Find("Unlit/Color"));
            Pointer.material.SetColor("_Color", PointerColor);
            NVRHelpers.LineRendererSetColor(Pointer, PointerColor, PointerColor);
        }

        Pointer.useWorldSpace = true;
    }
}
