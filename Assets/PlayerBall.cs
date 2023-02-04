using System;
using System.Collections;
using System.Collections.Generic;
using TheGrand.Utilities.Swizzles;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBall : MonoBehaviour
{
    public float force = 2.0F;
    public float angularDrag = 2.0F;

    public Rigidbody _rigidbody;
    public Transform _camera;
    private Vector2 _inputVector = Vector2.zero;
    private Vector3 _lastContactNormal = Vector3.up;

    private void OnValidate()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        var forward = _camera.forward.x0z().normalized;
        var right = _camera.transform.right.x0z().normalized;
        var movementVector = forward * _inputVector.y + right * _inputVector.x;
        var rotationAxis = Vector3.Cross(_lastContactNormal, movementVector).normalized;
        
        _rigidbody.angularVelocity += rotationAxis * (_inputVector.magnitude * force * Time.deltaTime);
        _rigidbody.angularDrag = angularDrag;
    }
}
