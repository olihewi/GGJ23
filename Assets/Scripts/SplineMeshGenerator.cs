using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SplineMeshGenerator : ScriptableObject
{
    public abstract Mesh GenerateMesh(SplineMeshData spline);
    public virtual Mesh GenerateCollider(SplineMeshData spline) => GenerateMesh(spline);
    public abstract Vector4 Bounds { get; }
}