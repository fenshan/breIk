using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This is the main script with every important variable
public class CameraScroller : MonoBehaviour
{
    public GameObject poppingUp;
    public static int CurrentAnxietyLevel; //Number of bad things activated and currently visible. On this depends the ending of the game and the scrolling tutorial
    public static int CurrentBlockingLevel; //Number of bad things deactivated and currently visible. On this depends PopUpBadThings
    public static int TotalBlockingLevel; //Number of bad things deactivated in total. On this depends the type of final and the next version of the game in the menu

    public const int TOO_MUCH_ANXIETY = 8; //if the current anxiety level reach the max anxiety allowed, the game ends
    public const int TUTORIAL_SCROLL_ANXIETY = 1; // todo 4; //anxiety level to trigger the scroll tutorial
    bool scrollTutorialAlreadyDone;
    bool tutorialCurrentlyPlaying;
    public Image dimScrollingTutorial;
    GameObject scrollingAnimation;

    public static bool canScroll;
    public static float currentScroll; //between [0, 1]
    public Vector3 CAMERA_MIN; //just the screen of the laptop. 0 scroll
    public Vector3 CAMERA_MAX; //whole room. 1 scroll
    public float speedManualScroll;

    AudioSource goodTheme, badTheme;
    public AudioClip good, bad;

    GlitchEffect shader;

    private void Start()
    {

        //global variables
        CurrentAnxietyLevel = 0;
        CurrentBlockingLevel = 0;
        TotalBlockingLevel = 0;
        UpdateTotalBlockingLevel(0);

        //scroll tutorial stuff
        scrollTutorialAlreadyDone = PlayerPrefs.HasKey("ScrollTutorial") ? true : false;
        tutorialCurrentlyPlaying = false;
        dimScrollingTutorial.enabled = false;
        scrollingAnimation = dimScrollingTutorial.rectTransform.GetChild(0).gameObject;
        scrollingAnimation.SetActive(false);

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
                    //todo end game
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
        //same exponential function of bad volume but inverted //exponential (1, 1) (0.8, 0.98) (0, 0.5)
        goodTheme.volume = 0.5f * (1 - Mathf.Exp(-3.78383f * currentScroll)) + 0.5f;

        //exponential volume 1.00021*e^(-3.78383*x) //exponential (1, 0) (0.8, 0.05) (0.7, 0.08) (0, 1)
        badTheme.volume = 0.6f * Mathf.Exp(-3.78383f * currentScroll) - 0.01f;
        if (!tutorialCurrentlyPlaying) badTheme.pitch = CalculatePitchBadTheme();
    }

    void SetGlitchEffect()
    {
        shader.intensity = Mathf.Lerp(0, 0.5f, 1 - currentScroll);

        if (currentScroll > 0.7f) shader.flipIntensity = 0;
        else shader.flipIntensity = Mathf.Lerp(0, 0.6f, 1 - currentScroll);

        if (currentScroll > 0.4f) shader.colorIntensity = 0;
        else shader.colorIntensity = Mathf.Lerp(0, 0.6f, 1 - currentScroll);
    }

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

        dimScrollingTutorial.enabled = true;
        dimScrollingTutorial.color = new Color(0, 0, 0, 0);

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
        dimScrollingTutorial.enabled = false;
        PopUpBadThings.canPopBadThings = true;
        tutorialCurrentlyPlaying = false;
    }

    void SetSlowing(float s, AudioSource[] audios)
    {
        Time.timeScale = s;
        dimScrollingTutorial.color = new Color(0, 0, 0, (1 - s) / 2.0f);
        foreach (AudioSource a in audios)
        {
            a.pitch = s;
            if (a == badTheme)
            {
                float alpha = 1.42857f * s - 0.428571f; //function (0.3, 0) (1, 1): 1.42857 x - 0.428571(linear)
                a.pitch = Mathf.Lerp(s, CalculatePitchBadTheme(), alpha);
            }
        }
    }

    float CalculatePitchBadTheme()
    {
        return Mathf.Lerp(0.9f, 2.0f, 1 - currentScroll);
    }


    /*
     * Input.GetAxis("Mouse ScrollWheel")
     * Input.mouseScrollDelta.y
     */
}
