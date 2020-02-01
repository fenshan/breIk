using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    int CAMERA_MIN = 5;
    int CAMERA_MAX = 25;
    float ratio = 15;

    private void Start()
    {
        Camera.main.orthographicSize = 5;
    }

    void Update()
    {
        AutomaticScroller();
        float cameraAux = Camera.main.orthographicSize - Input.mouseScrollDelta.y * Time.deltaTime * ratio;
        if (cameraAux >= CAMERA_MIN && cameraAux <= CAMERA_MAX) Camera.main.orthographicSize = cameraAux;









    }

    void AutomaticScroller()
    {

    }

    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
