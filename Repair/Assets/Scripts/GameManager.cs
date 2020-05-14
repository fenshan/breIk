using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This is the main script with every important variable
public class GameManager : MonoBehaviour
{
    public GameObject poppingUp;
    public static int CurrentAnxietyLevel; //Number of bad things activated and currently visible. On this depends the ending of the game and the scrolling tutorial
    public static int CurrentBlockingLevel; //Number of bad things deactivated and currently visible. On this depends PopUpBadThings
    public static int TotalBlockingLevel; //Number of bad things deactivated in total. On this depends the type of final and the next version of the game in the menu
    public static bool end;

    public const int TOO_MUCH_ANXIETY = 2;// todo 8; //if the current anxiety level reach the max anxiety allowed, the game ends
    public const int TUTORIAL_SCROLL_ANXIETY = 10; // todo 4; //anxiety level to trigger the scroll tutorial
    bool scrollTutorialAlreadyDone;
    bool tutorialCurrentlyPlaying;
    public GameObject scrollingAnimation;

    public static bool canScroll;
    public static float currentScroll; //between [0, 1]
    public Vector3 CAMERA_MIN; //just the screen of the laptop. 0 scroll
    public Vector3 CAMERA_MAX; //whole room. 1 scroll
    public float speedManualScroll;

    AudioSource goodTheme, badTheme;
    public AudioClip good, bad, breathing;

    GlitchEffect shader;
    public Image overlayImage;

    private void Start()
    {
        //global variables
        CurrentAnxietyLevel = 0;
        CurrentBlockingLevel = 0;
        TotalBlockingLevel = 0;
        UpdateTotalBlockingLevel(0);
        end = false;

        //scroll tutorial stuff
        scrollTutorialAlreadyDone = PlayerPrefs.HasKey("ScrollTutorial") ? true : false;
        tutorialCurrentlyPlaying = false;
        scrollingAnimation.SetActive(false);

        overlayImage.enabled = false;
        overlayImage.GetComponent<Animator>().enabled = false;

        //current scroll
        currentScroll = 1;
        canScroll = PlayerPrefs.HasKey("DragTutorial") ? true : false;

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

        StartCoroutine(UpdateAnxietyAndBlockingLevel());
    }

    void Update()
    {
        //UPDATE SCROLL
        if (Input.mouseScrollDelta.y != 0 && canScroll)
        {
            float cameraScrollAux = currentScroll - Input.mouseScrollDelta.y * Time.deltaTime * speedManualScroll;
            currentScroll = Mathf.Clamp(cameraScrollAux, 0, 1);
            Camera.main.transform.position = Vector3.Lerp(CAMERA_MIN, CAMERA_MAX, currentScroll);
            SetMusic();
            SetGlitchEffect();
        }

        //UPDATE ESC
        if (Input.GetKey(KeyCode.Escape))
        {
            AudioSource[] audios = FindObjectsOfType<AudioSource>();
            SetSlowing(1, audios);
            SceneManager.LoadScene("Menu");
        }
    }

    //UPDATE ANXIETY AND BLOCKING LEVEL
    IEnumerator UpdateAnxietyAndBlockingLevel()
    {
        while (true)
        {
            int anxiety = 0;
            int blocking = 0;
            FloatingObject[] objects = poppingUp.GetComponentsInChildren<FloatingObject>();
            foreach (FloatingObject o in objects)
            {
                if (o.bad && o.active) ++anxiety;
                else if (o.wasBad && o.active) ++blocking;
            }

            if (CurrentAnxietyLevel != anxiety)
            {
                CurrentAnxietyLevel = anxiety;
                //CHECK IF GAME ENDED
                if (CurrentAnxietyLevel >= TOO_MUCH_ANXIETY)
                {
                    StartCoroutine(EndingOfGame());
                }
                //CHECK IF THE SCROLL TUTORIAL NEEDS TO BE PLAYED
                else if (CurrentAnxietyLevel >= TUTORIAL_SCROLL_ANXIETY && !scrollTutorialAlreadyDone)
                {
                    scrollTutorialAlreadyDone = true;
                    tutorialCurrentlyPlaying = true;
                    StartCoroutine(ScrollTutorial());
                }
            }

            if (CurrentBlockingLevel != blocking)
            {
                CurrentBlockingLevel = blocking;
                //update time_left to next Bad Thing to pop up
                poppingUp.GetComponent<PopUpBadThings>().UpdateTimeLeft();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    public static void UpdateTotalBlockingLevel(int n)
    {
        TotalBlockingLevel += n;
        PlayerPrefs.SetInt("Version", TotalBlockingLevel);
    }

    void SetMusic()
    {
        goodTheme.volume = CalculateGoodThemeVolume();
        badTheme.volume = CalculateBadThemeVolume();
        if (!tutorialCurrentlyPlaying) badTheme.pitch = CalculateBadThemePitch();
    }

    void SetGlitchEffect()
    {
        shader.intensity = Mathf.Lerp(0, 0.5f, 1 - currentScroll);

        if (currentScroll > 0.7f) shader.flipIntensity = 0;
        else shader.flipIntensity = Mathf.Lerp(0, 0.6f, 1 - currentScroll);

        if (currentScroll > 0.4f) shader.colorIntensity = 0;
        else shader.colorIntensity = Mathf.Lerp(0, 0.6f, 1 - currentScroll);
    }

    #region scroll tutorial

    IEnumerator ScrollTutorial()
    {
        float startingScroll = currentScroll;
        float scrollingDelta = FloatingObject.RANGE + FloatingObject.RANGE / 2.0f; //the scrolling needed to end tutorial
        float finalSlowingIn = 0.3f;
        float finalSlowingOut = 1;
        float currentSlowing = finalSlowingOut;
        float speed = 1.0f; //speed of the slowing in and slowing out
        AudioSource[] audios = FindObjectsOfType<AudioSource>();

        PopUpBadThings.canPopBadThings = false;

        overlayImage.enabled = true;
        overlayImage.color = new Color(0, 0, 0, 0);

        //SLOW IN
        currentSlowing -= Time.deltaTime * speed;
        while (currentSlowing > finalSlowingIn)
        {
            SetSlowing(currentSlowing, audios);
            yield return null;
            currentSlowing -= Time.deltaTime * speed;
        }
        SetSlowing(finalSlowingIn, audios);

        scrollingAnimation.SetActive(true);

        //CONDITION TO END TUTORIAL
        while (Mathf.Abs(startingScroll - currentScroll) <= scrollingDelta && CurrentAnxietyLevel != 0)
        {
            yield return null;
        }

        scrollingAnimation.SetActive(false);
        PlayerPrefs.SetInt("ScrollTutorial", 1);

        //SLOW OUT
        currentSlowing += Time.deltaTime * speed;
        while (currentSlowing < finalSlowingOut)
        {
            SetSlowing(currentSlowing, audios);
            yield return null;
            currentSlowing += Time.deltaTime * speed;
        }
        SetSlowing(finalSlowingOut, audios);
        overlayImage.enabled = false;
        PopUpBadThings.canPopBadThings = true;
        tutorialCurrentlyPlaying = false;
    }

    void SetSlowing(float s, AudioSource[] audios)
    {
        Time.timeScale = s;
        overlayImage.color = new Color(0, 0, 0, (1 - s) / 2.0f);
        foreach (AudioSource a in audios)
        {
            a.pitch = s;
            if (a == badTheme)
            {
                float alpha = 1.42857f * s - 0.428571f; //function (0.3, 0) (1, 1): 1.42857 x - 0.428571(linear)
                a.pitch = Mathf.Lerp(s, CalculateBadThemePitch(), alpha);
            }
        }
    }

    #endregion scroll tutorial

    float CalculateGoodThemeVolume()
    {
        //same exponential function of bad volume but inverted //exponential (1, 1) (0.8, 0.98) (0, 0.5)

        return 0.5f * (1 - Mathf.Exp(-3.78383f * currentScroll)) + 0.5f;
    }

    float CalculateBadThemeVolume()
    {
        //exponential volume 1.00021*e^(-3.78383*x) //exponential (1, 0) (0.8, 0.05) (0.7, 0.08) (0, 1)
        return 0.6f * Mathf.Exp(-3.78383f * currentScroll) - 0.01f;
    }

    float CalculateBadThemePitch()
    {
        return Mathf.Lerp(0.9f, 2.0f, 1 - currentScroll);
    }

    IEnumerator EndingOfGame()
    {
        end = true;

        //duration of phases depending on TotalBlockingLevel
        //breathingTime 2.4 5.0 7.9 10.5
        float crisisTime, breathingTime;

        //todo calibrate crisis time and totalBlockingLevel
        if (TotalBlockingLevel == 0)
        {
            crisisTime = 0; breathingTime = 2.4f;
        }
        else if (TotalBlockingLevel == 1)//< 20) todo 
        {
            crisisTime = 4.0f; breathingTime = 5.0f;
        }
        else if (TotalBlockingLevel == 2)//< 80) todo
        {
            crisisTime = 7.0f; breathingTime = 7.9f;
        }
        else
        {
            crisisTime = 10f; breathingTime = 10.5f;
        }

        PopUpBadThings.canPopBadThings = false;
        canScroll = false;

        overlayImage.enabled = true;
        //todo make better crisis animation 
        if (crisisTime > 0) overlayImage.GetComponent<Animator>().enabled = true;

        float auxCrisisTime = 0;
        while (auxCrisisTime < crisisTime)
        {
            auxCrisisTime += Time.deltaTime;
            float alpha = auxCrisisTime / crisisTime;
            //the first half of the time, the bad sound increases
            if (alpha <= 0.5f)
            {
                goodTheme.volume = Mathf.Lerp(CalculateGoodThemeVolume(), 0, alpha * 2);
                badTheme.volume = Mathf.Lerp(CalculateBadThemeVolume(), 0.8f, alpha * 2);
                badTheme.pitch = Mathf.Lerp(CalculateBadThemePitch(), 2.0f, alpha * 2);
            }

            overlayImage.GetComponent<Animator>().speed = Mathf.Lerp(1, 1.7f, alpha);

            yield return null;
        }

        //stop every sound and start breathing sound 
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource a in audios) a.Stop();
        goodTheme.clip = breathing;
        goodTheme.volume = 1;
        goodTheme.Play();

        overlayImage.GetComponent<Animator>().enabled = false;
        float auxBreathingTime = 0;
        while (auxBreathingTime < breathingTime)
        {
            auxBreathingTime += Time.deltaTime;

            //first second with the fading in
            if (auxBreathingTime < 1)
            {
                overlayImage.color = new Color(0, 0, 0, auxBreathingTime);
            }
            else overlayImage.color = new Color(0, 0, 0, 1);

            yield return null;
        }

        SceneManager.LoadScene("Menu");
    }


    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
