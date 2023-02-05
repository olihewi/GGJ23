using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private bool _collected = false;
    private void Collect()
    {
        if (_collected) return;
        StageManager.Instance.Collect(this);
        _collected = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) Collect();
    }
}
