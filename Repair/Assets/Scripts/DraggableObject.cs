using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    static Canvas canvas;
    RectTransform parent;
    public RectTransform child;
    public static float RADIO_FOR_DISABLING_BAD = 70;

    static RectTransform currentImage;

    public void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        parent = GameObject.Find("PoppingUpLayers").GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentImage = Instantiate(child, parent);
        //Position
        currentImage.position = eventData.position;
        //Size
        float auxScale = Random.Range(1.5f, 3);
        currentImage.localScale = new Vector3 (auxScale, auxScale, auxScale);
        //Sprite
        currentImage.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        currentImage.GetComponent<Image>().SetNativeSize();
        currentImage.GetComponent<FloatingObject>().ScrollingPlace = CameraScroller.currentScroll;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentImage.position = eventData.position;
    }

    //Check if the placed good thing is disabling any bad thing
    public void OnPointerUp(PointerEventData eventData)
    {
        bool deactivate = false;
        foreach (FloatingObject f in parent.GetComponentsInChildren<FloatingObject>())
        {
            //only search between the active and bad items 
            if (f.active && f.bad)
            {
                //Debug.Log(Vector2.Distance(f.transform.position, currentImage.position));
                if (Vector2.Distance(f.transform.position, currentImage.position) < RADIO_FOR_DISABLING_BAD * canvas.scaleFactor)
                {
                    deactivate = true;
                    f.DeactivateAudio();
                    currentImage.GetComponent<FloatingObject>().ScrollingPlace = f.ScrollingPlace;
                    currentImage.GetComponent<FloatingObject>().CanFade = false;
                }
            }
        }

        currentImage = null;

        if (deactivate) SoundEffects.instance.DropAssetDeactivate();
        else SoundEffects.instance.DropAsset();
        parent.GetComponent<PopUpBadThings>().canPopBadThings = true;
    }

    public static void PutCurrentOnTop()
    {
        //if the player is currently dragging an object, put it on top of all other floatingObjects
        if (currentImage)
        {
            currentImage.SetAsLastSibling();
        }
    }
}