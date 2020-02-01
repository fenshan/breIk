using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragMenu : MonoBehaviour, IDragHandler
{
    public RectTransform SecondaryBars;
    Vector2 delta;
    public float MENU_SIZE;

    public void Start()
    {
        delta = SecondaryBars.position - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 AuxPos = eventData.position;

        //for not taking the menu outside the screen 
        Vector2 newPos = new Vector2(Mathf.Clamp(AuxPos.x, MENU_SIZE, Screen.width - MENU_SIZE), Mathf.Clamp(AuxPos.y, MENU_SIZE, Screen.height - MENU_SIZE));
        transform.position = newPos;
        SecondaryBars.position = newPos + delta;
    }

}
