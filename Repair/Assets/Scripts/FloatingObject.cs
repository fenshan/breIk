using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingObject : MonoBehaviour
{
    public static float RANGE = 0.07f;
    public float ScrollingPlace;
    public bool gif;

    void Update()
    {

        if (ScrollingPlace < CameraScroller.currentScroll + RANGE && ScrollingPlace > CameraScroller.currentScroll - RANGE)
        {
            gameObject.GetComponent<Image>().enabled = true;
            if (gif) gameObject.GetComponent<Animator>().enabled = true;
            //fade out fade in
            float aux = Mathf.Min(Mathf.Abs(CameraScroller.currentScroll + RANGE - ScrollingPlace), Mathf.Abs(-CameraScroller.currentScroll + RANGE + ScrollingPlace));

            if (aux < RANGE / 2.0f)
            {
                gameObject.GetComponent<Image>().color = new Color(1, 1, 1, aux * RANGE * 200);
            }
            else gameObject.GetComponent<Image>().color = Color.white;
        }
        else
        {
            gameObject.GetComponent<Image>().enabled = false;
            if (gif) gameObject.GetComponent<Animator>().enabled = false;
        }
    }
}
