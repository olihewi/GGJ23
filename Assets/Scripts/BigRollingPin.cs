using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRollingPin : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 10.0F;
    public Vector3 axis;
    void Update()
    {
        transform.eulerAngles += axis * speed * Time.deltaTime;
        //rb.angularVelocity = axis * speed;
        //rb.angularDrag = 0.0F;
    }
}
