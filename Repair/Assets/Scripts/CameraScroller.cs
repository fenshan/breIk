using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    public static float currentScroll; //between [0, 1]
    public Vector3 CAMERA_MIN; //0 scroll
    public Vector3 CAMERA_MAX; //1 scroll
    public float speedManualScroll;

    private void Start()
    {
        currentScroll = 0;
    }

    void Update()
    {
        //UPDATE SCROLL
        float cameraScrollAux = currentScroll + AutomaticScroller() + Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll; //TODO manual que solo se sume si es scroll in
        if (cameraScrollAux > 1) cameraScrollAux = 1;
        else if (cameraScrollAux < 0) cameraScrollAux = 0;
        Camera.main.transform.position = Vector3.Lerp(CAMERA_MIN, CAMERA_MAX, cameraScrollAux);
        currentScroll = cameraScrollAux;
    }

    float AutomaticScroller()
    {
        return 0;
    }

    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
