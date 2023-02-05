using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "The Grand/Procedural/Splines/Shape")]
public class SplineShapeGenerator : SplineMeshGenerator
{
    public float resolution = 0.5F;
    public float uvYScale = 1.0F;
    public bool seamless = true;
    public Vector3[] shape = Array.Empty<Vector3>();
    
    public override Mesh GenerateMesh(SplineMeshData spline)
    {
        float length = spline.LengthOS;
        float uvScale = seamless && spline.IsClosed ? Mathf.Round(length * uvYScale) / length : uvYScale;

        int numPoints = Mathf.CeilToInt(length / resolution);
        int numVerts = numPoints * shape.Length;

        Vector3[] vertices = new Vector3[numVerts];
        Vector2[] uvs = new Vector2[numVerts];
        int[] indices = new int[numVerts * 6];

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (numPoints - 1.0F);
            var pos = spline.EvaluatePositionOS(t);
            var scale = spline.EvaluateScale(t);
            var forward = spline.EvaluateForwardOS(t);
            var up = spline.EvaluateUpOS(t) * scale.y;
            var right = spline.EvaluateRight(up, forward) * scale.x;

            for (int j = 0; j < shape.Length; j++)
            {
                int index = i * shape.Length + j;
                vertices[index] = pos + right * shape[j].x + up * shape[j].y;
                uvs[index] = new Vector2(shape[j].z, t * length * uvScale);

                if (i < numPoints - 1 && j < shape.Length - 1)
                {
                    indices[index * 6]     = index;
                    indices[index * 6 + 1] = index + shape.Length;
                    indices[index * 6 + 2] = index + 1;
                    indices[index * 6 + 3] = index + 1;
                    indices[index * 6 + 4] = index + shape.Length;
                    indices[index * 6 + 5] = index + shape.Length + 1;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals();
        return mesh;
    }

    public override Vector4 Bounds =>
        new(
            shape.Min(x => x.x),
            shape.Max(x => x.x),
            shape.Min(x => x.y),
            shape.Max(x => x.y)
        );
}
