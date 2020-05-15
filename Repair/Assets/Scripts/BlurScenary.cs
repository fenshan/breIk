using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurScenary : MonoBehaviour
{
    public SpriteRenderer[] blurrableSprites;
    const float BLURRING_RANGE = 1.0f;

    void Update()
    {
        foreach(SpriteRenderer s in blurrableSprites)
        {
            if (Mathf.Abs(s.transform.position.z - Camera.main.transform.position.z)< BLURRING_RANGE)
            {
                //s.material.shader.
            }
        }        
    }
}
