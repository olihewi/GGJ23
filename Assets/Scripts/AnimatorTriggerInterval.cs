using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTriggerInterval : MonoBehaviour
{
    public Animator _animator;
    public string trigger = "Trigger";
    [Range(0.0F, 60.0F)] public float interval = 5.0F;
    [Range(0.0F, 60.0F)] public float delay = 0.0F;
    public int count = -1;

    private void OnValidate()
    {
        if (_animator != null) _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(PerformRoutine());
    }

    private IEnumerator PerformRoutine()
    {
        int _Trigger = Animator.StringToHash(trigger);
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < count || count < 0; i++)
        {
            _animator.SetTrigger(_Trigger);
            yield return new WaitForSeconds(interval);
        }
    }
}
