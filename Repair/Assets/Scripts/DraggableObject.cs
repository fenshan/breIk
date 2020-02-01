using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class DraggableObject : MonoBehaviour
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("I was clicked");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("I'm being dragged!");
    }



}
