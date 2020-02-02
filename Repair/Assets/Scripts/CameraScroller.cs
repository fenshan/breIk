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
    //public float speedAutomaticScroll;
    //int chanceToAutomaticScroll;

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
        //chanceToAutomaticScroll = 500;
    }

    void Update()
    {
        //UPDATE SCROLL
        //float cameraScrollAux = currentScroll + Time.deltaTime * AutomaticScroller();
        //if (Input.mouseScrollDelta.y > 0) cameraScrollAux -= Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll; //Manual Scroll is only available to Scroll In
        float cameraScrollAux = currentScroll  - Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll;
        if (cameraScrollAux > 1) cameraScrollAux = 1;
        else if (cameraScrollAux < 0) cameraScrollAux = 0;
        Camera.main.transform.position = Vector3.Lerp(CAMERA_MIN, CAMERA_MAX, cameraScrollAux);
        currentScroll = cameraScrollAux;
        SetMusic();
    }

    ////Automatic Scroll out (depending on CurrentAnxietyLevel)
    //float AutomaticScroller()
    //{
    //    //INCREASE AUTOMATICSPEED [logarithmic function: 1.649*log(0.263*x)*manualScroll]
    //    speedAutomaticScroll = 1.649f * Mathf.Log(0.263f * CurrentAnxietyLevel) * speedManualScroll;

    //    if (CurrentAnxietyLevel < 5) return 0;
    //    //CHANCE TO AUTOMATIC SCROLL
    //    if (Random.Range(0, chanceToAutomaticScroll) == 0)
    //    {
    //        //INCREASE CHANCE
    //        if (chanceToAutomaticScroll > 10) chanceToAutomaticScroll -= 40;
    //        else chanceToAutomaticScroll = 10;
    //        return speedManualScroll;
    //    }
    //    else return 0;
    //}

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
