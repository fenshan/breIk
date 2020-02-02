using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpBadThings : MonoBehaviour
{
    public GameObject[] BadThings;
    public float CurrentAnxietyLevel;
    static Canvas canvas;
    public float TIME;

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        StartCoroutine(PopUp());
    }

    IEnumerator PopUp()
    {
        while (true)
        {
            //Instantiate random bad thing
            GameObject bad = Instantiate(BadThings[Random.Range(0, BadThings.Length)], transform);
            //Random position
            Vector2 BAD_SIZE = bad.GetComponent<RectTransform>().sizeDelta;
            Debug.Log(BAD_SIZE);
            float x = Random.Range(BAD_SIZE.x / 2 * canvas.scaleFactor, Screen.width - BAD_SIZE.x / 2 * canvas.scaleFactor);
            float y = Random.Range(BAD_SIZE.y / 2 * canvas.scaleFactor, Screen.height - BAD_SIZE.y / 2 * canvas.scaleFactor);
            bad.GetComponent<RectTransform>().position = new Vector2(x, y);
            //Set the Scroll Layer of the floating object
            bad.GetComponent<FloatingObject>().ScrollingPlace = CameraScroller.currentScroll + Random.Range(-FloatingObject.RANGE/2.0f, 0);
            //set the time left until the next bad thing pops up TODO
            float time = TIME;
            yield return new WaitForSeconds(time);
        }
    }


}
