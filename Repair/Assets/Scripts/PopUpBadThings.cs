using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBadThings : MonoBehaviour
{
    public GameObject[] BadThings;
    public static bool canPopBadThings;
    float TIME_WHEN_SET;
    float time_left;

    private void Start()
    {
        canPopBadThings = PlayerPrefs.HasKey("DragTutorial") ? true : false;
        time_left = 3; //todo cambiar a 15 o así
    }

    private void Update()
    {
        if (!canPopBadThings) return;

        time_left -= Time.deltaTime;
        if (time_left <= 0)
        {
            //TIME
            float aux = TimeFunction(CameraScroller.CurrentBlockingLevel);
            //set the time left until the next bad thing pops up
            TIME_WHEN_SET = Random.Range(aux - aux / 2, aux + aux / 2);
            time_left = TIME_WHEN_SET;

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
            //DraggableObject.PutCurrentOnTop();
        }
    }


    public void UpdateTimeLeft()
    {
        //TIME
        float aux = TimeFunction(CameraScroller.CurrentBlockingLevel);
        float time_left_aux = Random.Range(aux - aux / 2, aux + aux / 2);

        time_left = time_left_aux - TIME_WHEN_SET + time_left;
        TIME_WHEN_SET = time_left_aux;
    }

    private float TimeFunction(int blocking)
    {
        return 1.3f + 18.24f * Mathf.Exp(-0.217f * blocking); //function: 1+18.24*e^(-0.217*x)
    }


}
