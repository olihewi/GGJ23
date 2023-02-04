using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsAnimator : MonoBehaviour
{
    public Rigidbody _rigidbody;

    private void OnValidate()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
    }

    private Vector3 _lastPos;
    private void FixedUpdate()
    {
        _rigidbody.velocity = (transform.position - _lastPos) / Time.deltaTime;
        _lastPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            collision.rigidbody.AddForceAtPosition(collision.impulse * 5.0F, collision.contacts[0].point, ForceMode.Impulse);
        }
    }
}
