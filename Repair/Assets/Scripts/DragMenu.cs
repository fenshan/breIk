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
        transform.position = eventData.position;
        SecondaryBars.position = eventData.position + delta;
    }

}
