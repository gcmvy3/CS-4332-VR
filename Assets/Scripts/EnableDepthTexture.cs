using UnityEngine;

[ExecuteInEditMode]
public class EnableDepthTexture : MonoBehaviour {

    private Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
    }

}