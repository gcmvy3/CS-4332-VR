using NewtonVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    private NVRPlayer player;
    public CustomNVRHand primaryHand;
    public CustomNVRHand secondaryHand;

    private Vector3 prevHandPosition = new Vector3();

    private VRTeleporter teleporter;
    private TerraformController terraformController;

    public float rotateThreshold = 0.8f;
    public float rotationAmount = 20;

    private bool justRotated = false;
    
    // Use this for initialization
    void Start () {
        player = GetComponent<NVRPlayer>();

        //Init teleporter
        teleporter = secondaryHand.teleporter;
        primaryHand.teleporter.gameObject.SetActive(false);

        //Init terraformer
        terraformController = primaryHand.terraformController;
        secondaryHand.terraformController.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {

        //Allow player to turn rapidly with secondary joystick
        NVRButtonInputs joystickValue;
        if(secondaryHand.Inputs.TryGetValue(NVRButtons.Axis0, out joystickValue)) {
            float joystickX = joystickValue.Axis.x;
            if(!justRotated && Mathf.Abs(joystickX) >= rotateThreshold) {

                float rotation = rotationAmount * Mathf.Sign(joystickX);

                player.transform.Rotate(new Vector3(0, rotation, 0));
                justRotated = true;
            }
            else if(justRotated && Mathf.Abs(joystickX) < rotateThreshold) {
                justRotated = false;
            }
        }

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
        else {
            //Allow player to teleport with secondary hand
            if (secondaryHand.UseButtonDown) {
                teleporter.ToggleDisplay(true);
            }
            else if(secondaryHand.UseButtonUp) {
                float playerHeight = player.transform.position.y;
                teleporter.ToggleDisplay(false);
                teleporter.Teleport();
                player.transform.position = new Vector3(player.transform.position.x, playerHeight, player.transform.position.z);
            }
        }

        //Terraform
        if(primaryHand.UseButtonDown && terraformController.isActiveAndEnabled) {
            terraformController.Click();
        }
    }
}
