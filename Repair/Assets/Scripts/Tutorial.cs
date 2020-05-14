using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is for the dragging tutorial
//The scrolling tutorial is in the GameManager script
public class Tutorial : MonoBehaviour
{
    public RectTransform[] dragTutorial;

    void Start()
    {
        //deactivate UI
        foreach (RectTransform r in dragTutorial)
        {
            r.gameObject.SetActive(false);
        }

        StartCoroutine(MenuAppears());
    }


    IEnumerator MenuAppears()
    {
        yield return null;//new WaitForSeconds(3.5f); todo
        //activate UI
        foreach (RectTransform r in dragTutorial)
        {
            r.gameObject.SetActive(true);
        }

        SoundEffects.instance.MenuAppears();
    }
}
