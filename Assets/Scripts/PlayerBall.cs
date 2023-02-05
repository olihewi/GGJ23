using System;
using System.Collections;
using System.Collections.Generic;
using TheGrand.Utilities.Swizzles;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerBall : MonoBehaviour
{
    public static PlayerBall Instance;

    public Transform modelRotation;
    
    public float force = 2.0F;

    public Rigidbody _rigidbody;
    [NonSerialized] public Vector2 _inputVector = Vector2.zero;
    private Vector3 _lastContactNormal = Vector3.up;
    private bool _touchingSurface = false;
    public float rotationFactor = 90.0F;
    private Vector2 _currentVelocity;

    private void OnValidate()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _inputVector = Vector2.SmoothDamp(_inputVector, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), ref _currentVelocity, 0.25F);
        var rotationAxis = Vector3.Cross(_lastContactNormal, _rigidbody.velocity);
        modelRotation.localRotation = Quaternion.Euler(rotationAxis * (Time.deltaTime * rotationFactor)) * modelRotation.localRotation;
    }

    private void FixedUpdate()
    {
        var forward = PlayerCamera.Instance.cameraTransform.forward.x0z().normalized;
        var right = PlayerCamera.Instance.cameraTransform.right.x0z().normalized;
        var movementVector = forward * _inputVector.y + right * _inputVector.x;
        var projectedMovement = Vector3.ProjectOnPlane(movementVector, _lastContactNormal).normalized;
        
        if (_touchingSurface)
            _rigidbody.AddForce(projectedMovement * force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _touchingSurface = true;
        if (collision.impulse.magnitude > 0.001F)
            _lastContactNormal = collision.impulse.normalized;
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        _touchingSurface = true;
        if (collisionInfo.impulse.magnitude > 0.001F)
            _lastContactNormal = collisionInfo.impulse.normalized;
    }

    private void OnCollisionExit(Collision other)
    {
        _touchingSurface = false;
        _lastContactNormal = Vector3.up;
        //_lastContactNormal = Vector3.zero;
    }
}
