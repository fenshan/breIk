using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Image fade;
    float timeFading;

    private void Start()
    {
        timeFading = 3.0f;
        StartCoroutine(SceneFadeIn());
    }

    //void Update()
    //{
    //    if (Input.GetKey(KeyCode.Escape))
    //    {
    //        Exit();
    //    }
    //}

    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    //public void Exit()
    //{
    //    Application.Quit();
    //}

    IEnumerator SceneFadeIn()
    {
        fade.enabled = true;
        float timeAux = 0;
        while (timeAux < timeFading)
        {
            timeAux += Time.deltaTime;

            //FADE OUT FUNCTION (0, 1) (0.8, 0.4) (1, 0)
            //0.368518 sin(x) + 2.84991 cos(x) - 1.84991 (periodic function)
            float alphaAux = timeAux / timeFading;//[0, 1]
            float alpha = 0.368518f * Mathf.Sin(alphaAux) + 2.84991f * Mathf.Cos(alphaAux) - 1.84991f;
            alpha = Mathf.Clamp(alpha, 0, 1);
            fade.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fade.enabled = false;
    }

}
