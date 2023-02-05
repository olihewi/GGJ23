using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private bool _collected = false;
    public AudioSource audioSource;
    private void Collect()
    {
        if (_collected) return;
        StageManager.Instance.Collect(this);
        _collected = true;
        audioSource.pitch = Mathf.Lerp(1.0F, 1.5F,
            (StageManager.Instance.collectibles % 8) / 8.0F);
        audioSource.Play();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) Collect();
    }
}
