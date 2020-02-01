using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DraggableType { meme, gif, sticker, none };

public class InteractableBar : MonoBehaviour
{
    public RectTransform secondaryBarPrefab;
    public float[] positionX;
    public float positionY;

    DraggableType currentType = DraggableType.none;
    RectTransform currentBar;

    public void PressedButton(int button)
    {
        DraggableType newType = (DraggableType) button;

        if (newType != currentType)
        {
            currentType = newType;
            Destroy(currentBar);
            currentBar = Instantiate(secondaryBarPrefab, GetComponent<RectTransform>());
            currentBar.position = currentBar.parent.position + new Vector3(positionX[(int)currentType], positionY, 0);
            //settear content de la barra
        }



    }

    public void Close()
    {
        Destroy(currentBar);
    }
}
