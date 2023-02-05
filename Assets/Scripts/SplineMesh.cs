using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
[ExecuteInEditMode]
public class SplineMesh : MonoBehaviour
{
    [HideInInspector] public MeshFilter meshFilter;
    [HideInInspector] public MeshCollider meshCollider;
    [HideInInspector] public MeshRenderer meshRenderer;
    public SplineMeshGenerator generator;
    public SplineContainer splineContainer;
    public SplineMeshData splineData = new();
    
#if UNITY_EDITOR
    private void Start()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.hideFlags = HideFlags.HideInInspector;
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                meshRenderer.sharedMaterial = primitive.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(primitive);
            }

        }
        if (splineContainer == null)
        {
            splineContainer = GetComponent<SplineContainer>();
        }
        
    }

    private void OnValidate()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (meshCollider == null)
            meshCollider = GetComponent<MeshCollider>();
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();
        splineData.Spline = splineContainer;
    }

    private void OnDestroy()
    {
        meshFilter.hideFlags = HideFlags.None;
    }
#endif
    
    [ContextMenu("Generate Mesh")]
    public void GenerateMesh()
    {
        if (generator != null)
        {
            splineData.Spline = splineContainer;
            meshFilter.sharedMesh = generator.GenerateMesh(splineData);
            if (meshCollider != null)
                meshCollider.sharedMesh = generator.GenerateCollider(splineData);
            Unwrapping.GenerateSecondaryUVSet(meshFilter.sharedMesh);
        }
    }
}
