using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragMenu : MonoBehaviour, IDragHandler
{
    public RectTransform SecondaryBars;
    Vector2 delta;

    public void Start()
    {
        delta = SecondaryBars.position - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 AuxPos = eventData.position;
        if (AuxPos.x >= 0 && AuxPos.x <= Screen.width && AuxPos.y >= 0 && AuxPos.y <= Screen.height)
        {
            transform.position = eventData.position;
            SecondaryBars.position = eventData.position + delta;
        }
    }

}
