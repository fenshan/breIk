using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragMenu : MonoBehaviour, IDragHandler
{
    public RectTransform SecondaryBars;
    public RectTransform Menu2;
    Vector2 deltaSecondaryBars;
    Vector2 deltaMenu2;

    Vector2 MENU_SIZE;
    static Canvas canvas;

    public void Start()
    {
        MENU_SIZE = GetComponent<RectTransform>().sizeDelta;

        canvas = FindObjectOfType<Canvas>();
        deltaSecondaryBars = (SecondaryBars.position - transform.position) / canvas.scaleFactor;
        deltaMenu2 = (Menu2.position - transform.position) / canvas.scaleFactor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 AuxPos = eventData.position;

        //for not taking the menu outside the screen 
        float x = Mathf.Clamp(AuxPos.x, MENU_SIZE.x / 2 * canvas.scaleFactor, Screen.width - MENU_SIZE.x / 2 * canvas.scaleFactor);
        float y = Mathf.Clamp(AuxPos.y, MENU_SIZE.y / 2 * canvas.scaleFactor, Screen.height - MENU_SIZE.y / 2 * canvas.scaleFactor);
        Vector2 newPos = new Vector2(x, y);
        transform.position = newPos;
        SecondaryBars.position = newPos + deltaSecondaryBars * canvas.scaleFactor;
        Menu2.position = newPos + deltaMenu2 * canvas.scaleFactor;
    }

}
