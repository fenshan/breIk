using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    public static int CurrentAnxietyLevel; //It goes up by one each time a bad thing pops up. It is exponential in time (setted from PopUpBadThings)
    public static float currentScroll; //between [0, 1]
    public Vector3 CAMERA_MIN; //just the screen of the laptop. 0 scroll
    public Vector3 CAMERA_MAX; //whole room. 1 scroll
    public float speedManualScroll;

    AudioSource goodTheme, badTheme;
    public AudioClip good, bad;

    private void Start()
    {
        CurrentAnxietyLevel = 0;
        currentScroll = 1;

        //Music themes
        goodTheme = GetComponents<AudioSource>()[0];
        goodTheme.clip = good;
        goodTheme.Play();
        badTheme = GetComponents<AudioSource>()[1];
        badTheme.clip = bad;
        badTheme.Play();
        //
    }

    void Update()
    {
        //UPDATE SCROLL
        float cameraScrollAux = currentScroll - Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll;
        cameraScrollAux = Mathf.Clamp(cameraScrollAux, 0, 1);
        Camera.main.transform.position = Vector3.Lerp(CAMERA_MIN, CAMERA_MAX, cameraScrollAux);
        currentScroll = cameraScrollAux;
        SetMusic();
    }

    void SetMusic()
    {
        goodTheme.volume = currentScroll;
        badTheme.volume = 1 - currentScroll;
        badTheme.pitch = Mathf.Lerp(0.9f, 1.1f, 1 - currentScroll);
    }

    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
