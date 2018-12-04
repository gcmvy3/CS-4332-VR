using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour {

    public GameObject item;
    public Transform itemPosition;
    public float rotateSpeed = 6;

	// Use this for initialization
	void Start () {
        ResetItemPosition();
	}
	
	// Update is called once per frame
	void Update () {
		if(item) {
            item.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
	}

    public void ResetItemPosition() {
        item.transform.parent = transform;
        item.transform.position = itemPosition.position;
        item.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Rigidbody itemBody = item.GetComponent<Rigidbody>();
        if (itemBody) {
            itemBody.velocity = Vector3.zero;
            itemBody.angularVelocity = Vector3.zero;
        }
    }
}
