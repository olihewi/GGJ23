using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;
    
    public Transform stageTilt;
    public Transform horizontalRotation, verticalRotation;
    public Transform topDownCamera;
    private Vector2 _rotation;
    public Camera cam;
    [Range(0.0F, 45.0F)] public float stageTiltAmount = 15.0F;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _rotation += new Vector2(
            Input.GetAxis("Mouse X") + 
            Input.GetAxis("LookHorizontal") * Time.deltaTime * 90.0F, 
            -Input.GetAxis("Mouse Y") + 
            Input.GetAxis("LookVertical") * Time.deltaTime * 90.0F);
        _rotation.y = Mathf.Clamp(_rotation.y, 25.0F, 80.0F);
        transform.position = PlayerBall.Instance.transform.position + Vector3.up;
        horizontalRotation.localRotation = Quaternion.Euler(0.0F, _rotation.x, 0.0F);
        verticalRotation.localRotation = Quaternion.Euler(_rotation.y, 0.0F, 0.0F);
        stageTilt.localRotation =
            Quaternion.Euler(-PlayerBall.Instance._inputVector.y * stageTiltAmount, 0.0F, PlayerBall.Instance._inputVector.x * stageTiltAmount);
        topDownCamera.localRotation = Quaternion.Euler(-PlayerBall.Instance._inputVector.y * 10.0F, 0.0F,
            PlayerBall.Instance._inputVector.x * 10.0F);
    }
}
