using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class TerraformItem : NVRInteractableItemClippable {

    public ItemHolder itemHolder;
    public CellType cellType;

    NVRPlayer player;
    CustomNVRHand customHand;


    public override void BeginInteraction(NVRHand hand) {
        base.BeginInteraction(hand);

        player = hand.Player;

        customHand = hand.GetComponent<CustomNVRHand>();
        customHand.terraformController.enabled = true;
        customHand.terraformController.SetCellType(cellType);
    }

    public override void EndInteraction(NVRHand hand) {
        base.EndInteraction(hand);
        itemHolder.ResetItemPosition();
        customHand.terraformController.SetCellType(CellType.Missing);
        customHand.terraformController.enabled = false;
    }
}
