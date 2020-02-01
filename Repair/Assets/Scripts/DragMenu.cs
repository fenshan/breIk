using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragMenu : MonoBehaviour, IDragHandler
{
    public RectTransform InteractableBar;
    Vector2 delta;

    public void Start()
    {
        delta = InteractableBar.position - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        InteractableBar.position = eventData.position + delta;
    }

}
