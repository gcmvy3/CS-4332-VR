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

    public bool terraformEnabled = true;
    Terraform terraform;

    Vector3 pointerOrigin;
    Vector3 pointerTarget;
    
    // Use this for initialization
    void Start () {
        player = GetComponent<NVRPlayer>();
        primaryHand = player.RightHand;
        secondaryHand = player.LeftHand;

        terraform = GameObject.FindObjectOfType<Terraform>();

        initPointer();
    }
	
	// Update is called once per frame
	void Update () {

        //Allow "pulling" movement by gripping with secondary hand
        if(secondaryHand.HoldButtonDown) {
            //Set anchor
            prevHandPosition = secondaryHand.CurrentPosition;
        }
        else if(secondaryHand.HoldButtonPressed) {
            //Move player
            Vector3 delta = secondaryHand.transform.position - prevHandPosition;
            player.transform.Translate(-delta);
        }

        //TODO maybe also have a button to enable pointer
        Pointer.enabled = terraformEnabled;

        if (Pointer.enabled == true) {
            updatePointer();
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

    private void updatePointer() {
        Pointer.material.SetColor("_Color", PointerColor);
        NVRHelpers.LineRendererSetColor(Pointer, PointerColor, PointerColor);
        NVRHelpers.LineRendererSetWidth(Pointer, PointerWidth, PointerWidth);

        pointerOrigin = primaryHand.transform.position;
        //TODO fix pointer angle
        //primaryHand.transform.RotateAround(primaryHand.transform.position, primaryHand.transform.forward, 45f);
        pointerTarget = primaryHand.transform.forward;
        //primaryHand.transform.RotateAround(primaryHand.transform.position, primaryHand.transform.forward, -45f);

        RaycastHit hitInfo;
        bool hit = Physics.Raycast(pointerOrigin, pointerTarget, out hitInfo, 1000);
        Vector3 endPoint;

        if (hit == true) {
            //1.01 extends the ray slightly to avoid landing on a hex boundary
            endPoint = hitInfo.point * 1.01f;
            if(terraformEnabled) {
                terraform.TouchCell(endPoint);
                if(primaryHand.UseButtonDown) {
                    terraform.PlaceCell(endPoint);
                }
                else if(primaryHand.HoldButtonDown && terraform.CanRemoveCell(endPoint)) {
                    terraform.RemoveCell(endPoint);
                }
            }
        }
        else {
            endPoint = pointerOrigin + (pointerTarget * 1000f);
        }

        Pointer.SetPositions(new Vector3[] { pointerOrigin, endPoint });
    }
}
