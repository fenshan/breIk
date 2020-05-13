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

    GlitchEffect shader;

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

        shader = GetComponent<GlitchEffect>();

        SetMusic();
        SetGlitchEffect();
    }

    void Update()
    {
        //UPDATE SCROLL
        if (Input.mouseScrollDelta.y != 0)
        {
            float cameraScrollAux = currentScroll - Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll;
            currentScroll = Mathf.Clamp(cameraScrollAux, 0, 1);
            Camera.main.transform.position = Vector3.Lerp(CAMERA_MIN, CAMERA_MAX, currentScroll);
            SetMusic();
            SetGlitchEffect();
        }
    }

    void SetMusic()
    {
        goodTheme.volume = Mathf.Lerp(0.5f, 1, currentScroll);

        //exponential volume 1.00021*e^(-3.78383*x) //exponential (1, 0) (0.8, 0.05) (0.7, 0.08) (0, 1)
        badTheme.volume = 0.35f * Mathf.Exp(-3.78383f * currentScroll) - 0.004f;
        badTheme.pitch = Mathf.Lerp(0.9f, 1.1f, 1 - currentScroll);
    }

    void SetGlitchEffect()
    {
        shader.intensity = Mathf.Lerp(0, 0.5f, 1 - currentScroll);

        if (currentScroll > 0.7f) shader.flipIntensity = 0;
        else shader.flipIntensity = Mathf.Lerp(0, 0.5f, 1 - currentScroll);

        if (currentScroll > 0.4f) shader.colorIntensity = 0;
        else shader.colorIntensity = Mathf.Lerp(0, 0.5f, 1 - currentScroll);
    }


    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
