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

    public Vector2 MENU_SIZE;
    public static Canvas canvas;

    public void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        deltaSecondaryBars = (SecondaryBars.position - transform.position)/ canvas.scaleFactor;
        deltaMenu2 = (Menu2.position - transform.position)/ canvas.scaleFactor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 AuxPos = eventData.position;

        //for not taking the menu outside the screen 
        Vector2 newPos = new Vector2(Mathf.Clamp(AuxPos.x, 0, Screen.width - MENU_SIZE.x * canvas.scaleFactor), Mathf.Clamp(AuxPos.y, 0, Screen.height - MENU_SIZE.y * canvas.scaleFactor));
        transform.position = newPos;
        SecondaryBars.position = newPos + deltaSecondaryBars * canvas.scaleFactor;
        Menu2.position = newPos + deltaMenu2 * canvas.scaleFactor;
    }

}
