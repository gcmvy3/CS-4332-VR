using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class CustomNVRHand : NVRHand {
    [HideInInspector]
    public bool collisionEnabled = true;

    public VRTeleporter teleporter;
    public TerraformController terraformController;

    public bool DisableCollision() {

        NVRPhysicalController controller = gameObject.GetComponent<NVRPhysicalController>();
        if(controller) {

            GameObject physicalController = controller.PhysicalController;

            Collider[] colliders = physicalController.GetComponentsInChildren<Collider>(true);

            for (int i = 0; i < colliders.Length; i++) {
                colliders[i].enabled = false;
            }

            collisionEnabled = false;
        }

        return !collisionEnabled;
    }
}
