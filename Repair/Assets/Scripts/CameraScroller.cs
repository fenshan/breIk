using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    public float CurrentAnxietyLevel; //TODO que solo pueda subir a partir del primer sticker puesto
    public static float currentScroll; //between [0, 1]
    public Vector3 CAMERA_MIN; //just the screen of the laptop. 0 scroll
    public Vector3 CAMERA_MAX; //whole room. 1 scroll
    public float speedManualScroll;
    public float speedAutomaticScroll;

    AudioSource goodTheme, badTheme;
    public AudioClip good, bad;

    private void Start()
    {
        CurrentAnxietyLevel = 0;
        //Music themes
        goodTheme = GetComponents<AudioSource>()[0];
        goodTheme.clip = good;
        goodTheme.Play();
        badTheme = GetComponents<AudioSource>()[1];
        badTheme.clip = bad;
        badTheme.Play();
        //
        currentScroll = 0;
    }

    void Update()
    {
        //UPDATE SCROLL
        float cameraScrollAux = currentScroll + Time.deltaTime * AutomaticScroller() - Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll; //TODO manual que solo se sume si es scroll in
        if (cameraScrollAux > 1) cameraScrollAux = 1;
        else if (cameraScrollAux < 0) cameraScrollAux = 0;
        Camera.main.transform.position = Vector3.Lerp(CAMERA_MIN, CAMERA_MAX, cameraScrollAux);
        currentScroll = cameraScrollAux;
        SetMusic();
    }

    float AutomaticScroller() //TODO automatic scroll out
    {
        //o 0 o la speed
        //aumenta la speed
        return 0;
    }

    void SetMusic()
    {
        goodTheme.volume = 1 - currentScroll;
        badTheme.volume = currentScroll;
        badTheme.pitch = Mathf.Lerp(0.9f, 1.1f, currentScroll);
    }

    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
