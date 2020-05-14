using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioClip dropAsset, dropAssetDeactivate, menuAppears, menu, pickAsset;

    AudioSource source;
    public static SoundEffects instance;

    private void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    public void DropAsset()
    {
        source.clip = dropAsset;
        source.Play();
    }

    public void DropAssetDeactivate()
    {
        source.clip = dropAssetDeactivate;
        source.Play();
    }

    public void MenuAppears()
    {
        source.clip = menuAppears;
        source.Play();
    }

    public void Menu()
    {
        source.clip = menu;
        source.Play();
    }

    public void PickAsset()
    {
        source.clip = pickAsset;
        source.Play();
    }
}
