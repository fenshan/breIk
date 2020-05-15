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

    public const int TOO_MUCH_ANXIETY = 2; //if the current anxiety level reach the max anxiety allowed, the game ends
    public const int TUTORIAL_SCROLL_ANXIETY = 4; //anxiety level to trigger the scroll tutorial
    public static bool scrollTutorialAlreadyDone;
    public static bool tutorialCurrentlyPlaying;
    public GameObject scrollingAnimation;

    public static bool canScroll;
    public static float currentScroll; //between [0, 1]
    public Vector3 CAMERA_MIN; //just the screen of the laptop. 0 scroll
    public Vector3 CAMERA_MAX; //whole room. 1 scroll
    public float speedManualScroll;

    AudioSource goodTheme, badTheme, whistleSource;
    public AudioClip good, bad, breathing, whistle;

    GlitchEffect shader;
    public Image overlayImage, endingScreenshot;

    public GameObject[] UIToHideAtEnding;

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
        endingScreenshot.enabled = false;

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
        whistleSource = GetComponents<AudioSource>()[2];
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

    #region music
    void SetMusic()
    {
        goodTheme.volume = CalculateGoodThemeVolume();
        badTheme.volume = CalculateBadThemeVolume();
        if (!tutorialCurrentlyPlaying) badTheme.pitch = CalculateBadThemePitch();
    }

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
    #endregion music

    #region glitch
    void SetGlitchEffect()
    {
        shader.intensity = CalculateShaderIntensity();
        shader.flipIntensity = CalculateShaderFlip();
        shader.colorIntensity = CalculateShaderColor();
    }

    float CalculateShaderIntensity()
    {
        return Mathf.Lerp(0, 0.5f, 1 - currentScroll);
    }

    float CalculateShaderFlip()
    {
        if (currentScroll > 0.7f) return 0;
        return Mathf.Lerp(0, 0.6f, 1 - currentScroll);
    }

    float CalculateShaderColor()
    {
        if (currentScroll > 0.4f) return 0;
        return Mathf.Lerp(0, 0.6f, 1 - currentScroll);
    }
    #endregion glitch

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

    #region ending 
    IEnumerator EndingOfGame()
    {
        end = true;
        PopUpBadThings.canPopBadThings = false;
        canScroll = false;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();

        //duration of phases depending on TotalBlockingLevel
        //breathingTime 2.4 5.0 7.9 10.5
        float crisisTime, breathingTime, silenceTime, initialVisualGlitchTime = 1.0f;
        
        if (TotalBlockingLevel == 0)
        {
            crisisTime = 0; breathingTime = 2.4f; silenceTime = 1.5f;
        }
        else if (TotalBlockingLevel < 15)
        {
            crisisTime = 3.0f; breathingTime = 5.0f; silenceTime = 2;
        }
        else if (TotalBlockingLevel < 100) 
        {
            crisisTime = 6.0f; breathingTime = 7.9f; silenceTime = 2.5f;
        }
        else
        {
            crisisTime = 8.0f; breathingTime = 10.5f; silenceTime = 3;
        }

        #region crisis
        if (crisisTime > 0)
        {
            StartCoroutine(FreezeScreen());
            StartCoroutine(PlayWhistle(crisisTime, breathingTime));
            foreach (AudioSource a in audios)
            {
                if (a != whistleSource)
                {
                    StartCoroutine(FadeOutAudio(a, crisisTime / 2.0f));
                }
            }            
            StartCoroutine(CrisisShaders(initialVisualGlitchTime, crisisTime - initialVisualGlitchTime));

            yield return new WaitForSeconds(crisisTime);
        }
        #endregion crisis

        #region breathing

        //stop every sound and play breathing sound         
        foreach (AudioSource a in audios)
        {
            if (a != whistleSource) a.Stop();
        }
        StartCoroutine(PlayBreathing(breathingTime));
        StartCoroutine(OverlayFadingToBlack(1));
        yield return new WaitForSeconds(breathingTime + silenceTime); // X seconds breathing and whistle fading out + silence

        #endregion breathing

        SceneManager.LoadScene("Menu");
    }

    IEnumerator FreezeScreen()
    {
        //Take screenshot
        yield return new WaitForEndOfFrame();
        //ScreenCapture.CaptureScreenshot("Assets/Screenshots/" + Random.Range(0, 297498378) + ".png"); //todo quitar
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture(2);
        Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        endingScreenshot.sprite = s;
        endingScreenshot.enabled = true;
        //hide UI
        foreach (GameObject g in UIToHideAtEnding)
        {
            g.SetActive(false);
        }
    }

    IEnumerator FadeOutAudio(AudioSource a, float time)
    {
        float startingVolume = a.volume;

        float auxTime = 0;
        while (auxTime < time)
        {
            auxTime += Time.deltaTime;
            float alpha = auxTime / time;
            a.volume = Mathf.Lerp(startingVolume, 0, alpha);
            yield return null;
        }
        a.volume = 0;
    }

    IEnumerator PlayWhistle(float crisisTime, float breathingTime)
    {
        float startingWhistleVolume = 0.5f;
        //Start
        whistleSource.clip = whistle;
        whistleSource.volume = startingWhistleVolume;
        whistleSource.Play();
        yield return new WaitForSeconds(crisisTime);
        //Fade out
        StartCoroutine(FadeOutAudio(whistleSource, breathingTime / 2.0f));
    }

    IEnumerator PlayBreathing(float time)
    {
        goodTheme.clip = breathing;
        goodTheme.volume = 1;
        goodTheme.Play();
        yield return new WaitForSeconds(time);
        goodTheme.Stop();
    }

    IEnumerator OverlayFadingToBlack(float time)
    {
        overlayImage.enabled = true;
        overlayImage.GetComponent<Animator>().enabled = false;

        float auxTime = 0;
        while (auxTime < time)
        {
            auxTime += Time.deltaTime;
            overlayImage.color = new Color(0, 0, 0, auxTime / time);
            yield return null;
        }
        overlayImage.color = new Color(0, 0, 0, 1);
    }

    IEnumerator CrisisShaders(float timeMAX, float timeINCR)
    {
        //MAXIMUM
        float auxTime = 0;
        while (auxTime < timeMAX)
        {
            auxTime += Time.deltaTime;
            shader.intensity = shader.flipIntensity = shader.colorIntensity = 1;
            yield return null;
        }

        //PROGRESSIVELY INCREASING
        auxTime = 0;
        while (auxTime < timeINCR)
        {
            auxTime += Time.deltaTime;
            float alpha = auxTime / timeINCR;
            shader.intensity = Mathf.Lerp(CalculateShaderIntensity(), 1, alpha);
            shader.flipIntensity = Mathf.Lerp(CalculateShaderFlip(), 1, alpha);
            shader.colorIntensity = Mathf.Lerp(CalculateShaderColor(), 1, alpha);
            yield return null;
        }
    }

    #endregion ending
}
