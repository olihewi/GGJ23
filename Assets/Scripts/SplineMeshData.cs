using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[Serializable]
public class SplineMeshData
{
    public SplineContainer Spline
    {
        get => spline;
        set
        {
            spline = value;
            Array.Resize(ref knotScales, spline.Spline.Count);
            for (int i = 0; i < knotScales.Length; i++)
            {
                if (knotScales[i] == Vector2.zero)
                    knotScales[i] = Vector2.one;
            }
        }
    }
    public SplineContainer spline;
    public Vector2[] knotScales;

    public float LengthWS => spline.CalculateLength();
    public float LengthOS => spline.Spline.GetLength();
    public int Count => spline.Spline.Count;
    public IEnumerable<BezierKnot> Knots => spline.Spline.Knots;

    public bool IsClosed => spline.Spline.Closed;

    public Vector3 EvaluatePositionWS(float time) => spline.EvaluatePosition(time);
    public Vector3 EvaluateForwardWS(float time) => ((Vector3)spline.EvaluateTangent(time)).normalized;
    public Vector3 EvaluateUpWS(float time) => ((Vector3)spline.EvaluateUpVector(time)).normalized;
    public Vector3 EvaluateRightWS(float time) => Quaternion.AngleAxis(90.0F, EvaluateUpWS(time)) * EvaluateForwardWS(time);
    public Vector3 EvaluateAccelerationWS(float time) => spline.EvaluateAcceleration(time);
    
    public Vector3 EvaluatePositionOS(float time) => spline.Spline.EvaluatePosition(time);

    public Vector3 EvaluateForwardOS(float time)
    {
        Vector3 forward = spline.Spline.EvaluateTangent(time);
        if (forward.magnitude > 0.0F) return forward.normalized;
        forward = EvaluatePositionOS(time + 0.1F) - EvaluatePositionOS(time);
        if (forward.magnitude > 0.0F) return forward.normalized;
        forward = EvaluatePositionOS(time) - EvaluatePositionOS(time - 0.1F);
        return forward.normalized;
    }
    public Vector3 EvaluateUpOS(float time) => ((Vector3)spline.Spline.EvaluateUpVector(time)).normalized;
    public Vector3 EvaluateRightOS(float time) => Quaternion.AngleAxis(90.0F, EvaluateUpOS(time)) * EvaluateForwardOS(time);
    public Vector3 EvaluateRight(Vector3 up, Vector3 forward) => Quaternion.AngleAxis(90.0F, up) * forward;
    public Vector3 EvaluateAccelerationOS(float time) => spline.Spline.EvaluateAcceleration(time);

    public Vector2 EvaluateScale(float time)
    {
        time = Mathf.Clamp01(time);
        int numKnots = IsClosed ? knotScales.Length : knotScales.Length - 1;
        float indexF = time * numKnots;
        int index = Mathf.FloorToInt(indexF);
        int index2 = (index + 1) % knotScales.Length;
        float delta = indexF - index;
        return Vector2.Lerp(
            knotScales[index],
            knotScales[index2], 
            delta);
    }
    public float EvaluateCurvature(float time) => spline.Spline.EvaluateCurvature(time);
    public Vector3 EvaluateCurvatureCenter(float time) => spline.Spline.EvaluateCurvatureCenter(time);
}