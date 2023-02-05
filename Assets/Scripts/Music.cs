using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static Music Instance;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            GetComponent<AudioSource>().Play();
            DontDestroyOnLoad(gameObject);
        }
        else DestroyImmediate(this);
    }
}
