using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour
{
    void Start()
    {
        int version = 0;
        if (PlayerPrefs.HasKey("Version"))
        {
            version = PlayerPrefs.GetInt("Version");
        }

        string s = "v." + ((version / 100) % 100).ToString() + '.' + ((version / 10) % 10).ToString() + '.' + (version % 10).ToString();

        GetComponent<Text>().text = s;
    }
}
