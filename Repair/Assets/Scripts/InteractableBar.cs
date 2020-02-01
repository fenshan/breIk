using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DraggableType { gif, sticker, none };

public class InteractableBar : MonoBehaviour
{
    public RectTransform secondaryBarPrefab;
    public float positionX;
    public float positionY;

    public Sprite[] backgroundsSecondaryBar;
    public GameObject StickerButton;
    public GameObject[] GifButtons;

    DraggableType currentType = DraggableType.none;
    RectTransform[] Bars;

    void Start()
    {
        #region initializate the 2 secondary menu bars
        Bars = new RectTransform[2];

        for (int i = 0; i < Bars.Length; ++i)
        {
            Bars[i] = Instantiate(secondaryBarPrefab, GetComponent<RectTransform>());
            Bars[i].position = transform.position + new Vector3(positionX, positionY, 0) * transform.localScale.x;
            Bars[i].GetComponent<Image>().sprite = backgroundsSecondaryBar[i];
            Bars[i].gameObject.SetActive(false);
        }

        //GIF, set content of the secondary bar
        Transform content0 = Bars[(int)DraggableType.gif].Find("Viewport").Find("Content");
        foreach (GameObject g in GifButtons)
        {
            GameObject sticker = Instantiate(g, content0);
            sticker.GetComponent<Image>().SetNativeSize();
        }

        //STICKERS, set content of the secondary bar
        Transform content1 = Bars[(int)DraggableType.sticker].Find("Viewport").Find("Content");
        Sprite[] sprites = Resources.LoadAll<Sprite>("Stickers"); //Application.dataPath +
        foreach (Sprite s in sprites)
        {
            GameObject sticker = Instantiate(StickerButton, content1);
            sticker.GetComponent<Image>().sprite = s;
            sticker.GetComponent<Image>().SetNativeSize();
        }

        #endregion initializate the 3 secondary menu bars
    }


    public void PressedButton(int button)
    {
        DraggableType newType = (DraggableType)button;
        if (currentType != DraggableType.none) Bars[(int)currentType].gameObject.SetActive(false);

        if (newType != currentType)
        {
            currentType = newType;
            Bars[(int)currentType].gameObject.SetActive(true);
        }
        else currentType = DraggableType.none;

    }

    public void Close()
    {
        Bars[(int)currentType].gameObject.SetActive(false);
    }
}
