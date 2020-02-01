using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    RectTransform parent;
    public RectTransform child;

    RectTransform currentImage;

    public void Start()
    {
        parent = GameObject.Find("PoppingUpLayers").GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("I was clicked");

        currentImage = Instantiate(child, parent);
        currentImage.position = eventData.position;
        currentImage.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        currentImage.GetComponent<Image>().SetNativeSize();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("I'm being dragged!");

        currentImage.position = eventData.position;
    }
}
