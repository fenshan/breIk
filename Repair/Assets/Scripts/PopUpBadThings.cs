using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBadThings : MonoBehaviour
{
    public GameObject[] BadThings;
    public bool canPopBadThings;
    float STARTING_TIME;
    float TIME;

    private void Start()
    {
        canPopBadThings = false;
        StartCoroutine(PopUp());
    }

    IEnumerator PopUp()
    {
        while (!canPopBadThings) yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(5f);
        while (true)
        {
            //Instantiate random bad thing
            GameObject bad = Instantiate(BadThings[Random.Range(0, BadThings.Length)], transform);
            bad.GetComponent<Image>().SetNativeSize();
            //Random position
            Vector2 BAD_SIZE = bad.GetComponent<RectTransform>().sizeDelta;
            float x = Random.Range(BAD_SIZE.x / 2, Screen.width - BAD_SIZE.x / 2);
            float y = Random.Range(BAD_SIZE.y / 2, Screen.height - BAD_SIZE.y / 2);
            bad.GetComponent<RectTransform>().position = new Vector2(x, y);
            //Size
            bad.transform.localScale = new Vector3(2, 2, 2);
            //Rotation
            bad.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15, 15));
            //Set the Scroll Layer of the floating object
            bad.GetComponent<FloatingObject>().ScrollingPlace = CameraScroller.currentScroll + Random.Range(-FloatingObject.RANGE / 2.0f, 0);
            //If the player is currently dragging a good floating object, put it on top
            DraggableObject.PutCurrentOnTop();

            //TIME
            ++CameraScroller.CurrentAnxietyLevel; //INCREMENT ANXIETY LEVEL EACH TIME SOMETHING BAD POPS UP
            float aux = CameraScroller.CurrentAnxietyLevel;
            TIME = 1.3f + 18.24f * Mathf.Exp(-0.217f * aux); //function: 1+18.24*e^(-0.217*x)
            //set the time left until the next bad thing pops up
            float time = Random.Range(TIME - TIME / 2, TIME + TIME / 2);
            yield return new WaitForSeconds(time);
        }
    }


}
