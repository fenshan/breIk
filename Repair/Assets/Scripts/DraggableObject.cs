using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    static Canvas canvas;
    RectTransform parent0;
    RectTransform parent1;
    public RectTransform child;
    public const float RADIO_FOR_DISABLING_BAD = 90;

    static RectTransform currentImage;
    float currentCameraScroll;

    public void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        parent0 = GameObject.Find("Dragging").GetComponent<RectTransform>();
        parent1 = GameObject.Find("PoppingUpLayers").GetComponent<RectTransform>();
    }

    public void Update()
    {
        if (currentImage && currentCameraScroll != GameManager.currentScroll)
        {
            currentCameraScroll = GameManager.currentScroll;
            currentImage.GetComponent<FloatingObject>().ScrollingPlace = currentCameraScroll + Random.Range(-FloatingObject.RANGE / 2.0f, 0);
        }

        if (currentImage && GameManager.end)
        {
            currentImage.SetParent(parent1);
            currentImage.SetAsLastSibling();
            currentImage = null;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentImage = Instantiate(child, parent0);
        //Position
        currentImage.position = eventData.position;
        //Size
        float auxAlpha = Random.Range(0f, 1f);
        //x=[0,0.8) y=[1.1, 1.5] linear function 0.5 x + 1.1 (linear)
        //x=[0.8, 1] y=[1.5, 2.0] periodic function -1.1949 sin(x) - 4.14505 cos(x) + 5.24505 (periodic)
        float auxScale;
        if (auxAlpha < 0.8) auxScale = 0.5f * auxAlpha + 1.1f;
        else auxScale = -1.1949f * Mathf.Sin(auxAlpha) - 4.14505f * Mathf.Cos(auxAlpha) + 5.24505f;

        currentImage.localScale = new Vector3(auxScale, auxScale, auxScale);
        //Sprite
        currentImage.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        currentImage.GetComponent<Image>().SetNativeSize();
        currentCameraScroll = GameManager.currentScroll;
        currentImage.GetComponent<FloatingObject>().ScrollingPlace = currentCameraScroll + Random.Range(-FloatingObject.RANGE / 2.0f, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentImage) currentImage.position = eventData.position;
    }

    //Check if the placed good thing is disabling any bad thing
    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.end) return;

        currentImage.SetParent(parent1);
        currentImage.SetAsLastSibling();

        bool deactivate = false;
        foreach (FloatingObject f in parent1.GetComponentsInChildren<FloatingObject>())
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
                    GameManager.UpdateTotalBlockingLevel(1);
                }
            }
        }

        currentImage = null;

        if (deactivate) SoundEffects.instance.DropAssetDeactivate();
        else SoundEffects.instance.DropAsset();
        PopUpBadThings.canPopBadThings = true;
        if (!GameManager.canScroll)
        {
            GameManager.canScroll = true;
            PlayerPrefs.SetInt("DragTutorial", 1);
        }

    }

    //public static void PutCurrentOnTop()
    //{
    //    //if the player is currently dragging an object, put it on top of all other floatingObjects
    //    if (currentImage)
    //    {
    //        currentImage.SetAsLastSibling();
    //    }
    //}
}