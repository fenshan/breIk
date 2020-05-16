using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurScenary : MonoBehaviour
{
    public float minBlurring, maxBlurring;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float camera_z = Camera.main.transform.position.z;
        if (camera_z > minBlurring && camera_z < maxBlurring)
        {
            float alpha = (camera_z - minBlurring) / (maxBlurring - minBlurring);
            Debug.Log(alpha);
            float blur = Mathf.Lerp(0, 15, alpha);
            sprite.material.SetFloat("radius", blur);
        }
        else
        {
            sprite.material.SetFloat("radius", 0);
        }
    }
}
