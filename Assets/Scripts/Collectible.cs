using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void Collect()
    {
        StageManager.Instance.Collect(this);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collect();
    }
}
