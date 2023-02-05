using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheGrand.Utilities.Swizzles;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "The Grand/Procedural/Splines/Deformed Mesh")]
public class SplineDeformedMeshGenerator : SplineMeshGenerator
{
    [Serializable]
    public class MeshColliderPair
    {
        public Mesh mesh;
        public Mesh collider;
    }

    public MeshColliderPair startMesh;
    public MeshColliderPair[] meshes;
    public MeshColliderPair endMesh;
    
    [Range(-1, 1)] public float spacing = 0.0F;
    public bool seamless = true;
    public bool useWorldUp = false;
    public bool useWorldForward = false;
    public bool stairMode = false;
    public bool knotSnapping = false;

    private Mesh GenerateMesh(SplineMeshData spline, bool collider)
    {
        var start = collider ? startMesh.collider : startMesh.mesh;
        var cores = (collider ? meshes.Select(x => x.collider) : meshes.Select(x => x.mesh)).ToArray();
        var end = collider ? endMesh.collider : endMesh.mesh;
        float averageLength = cores.Average(x => x.bounds.size.z);
        
        float length = spline.LengthOS;
        var meshSequence = new List<Mesh>();
        float sequenceLength = 0.0F;
        if (!spline.IsClosed && start != null)
        {
            meshSequence.Add(start);
            sequenceLength += start.bounds.size.z + spacing;
            sequenceLength -= spacing;
        }
        float remainingLength = length - (!spline.IsClosed && end != null ? end.bounds.size.z + spacing : 0.0F);
        while (sequenceLength + averageLength / 2.0F < remainingLength)
        {
            var randomMesh = cores.ElementAt(Random.Range(0, cores.Length));
            meshSequence.Add(randomMesh);
            sequenceLength += randomMesh.bounds.size.z + spacing;
        }
        
        if (!spline.IsClosed)
        {
            var e = end;
            if (e == null && start != null)
            {
                var tris = start.triangles;
                for (int i = 0; i < tris.Length; i += 3)
                {
                    tris[i + 1] += tris[i + 2];
                    tris[i + 2] = tris[i + 1] - tris[i + 2];
                    tris[i + 1] -= tris[i + 2];
                }
                e = new Mesh()
                {
                    vertices = start.vertices.Select(x =>
                    {
                        x.z = start.bounds.max.z - x.z;
                        return x;
                    }).ToArray(),
                    triangles = tris,
                    uv = start.uv,
                    normals = start.normals.Select(x =>
                    {
                        x.z = -x.z;
                        return x;
                    }).ToArray()
                };
            }
            if (e != null)
            {
                meshSequence.Add(e);
                sequenceLength += e.bounds.size.z + spacing;
            }
        }
        
        Vector3[] vertices = new Vector3[meshSequence.Sum(x => x.vertexCount)];
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        int[] indices = new int[meshSequence.Sum(x => x.triangles.Length)];

        int vertexOffset = 0, indexOffset = 0;
        float t = 0.0F;
        foreach (var mesh in meshSequence)
        {
            var vertexCount = mesh.vertexCount;
            var meshVertices = new Vector3[vertexCount];
            var meshNormals = new Vector3[vertexCount];
            var t2 = t + (mesh.bounds.size.z) / sequenceLength;
            
            for (int i = 0; i < vertexCount; i++)
            {
                var v = mesh.vertices[i];
                v.z -= mesh.bounds.min.z;
                var n = mesh.normals[i];
                var t1 = Mathf.Lerp(t, t2, v.z / (mesh.bounds.size.z));
                if (spline.IsClosed) t1 %= 1.0F;
                var splinePos = spline.EvaluatePositionOS(t1);
                if (stairMode)
                {
                    splinePos.y = spline.EvaluatePositionOS(t).y;
                }
                var forward = spline.EvaluateForwardOS(t1);
                if (useWorldForward) forward = forward.x0z().normalized;
                var up = useWorldUp ? Vector3.up : spline.EvaluateUpOS(t1);
                var right = spline.EvaluateRight(up, forward);
                var scale = spline.EvaluateScale(t1);
                right *= scale.x;
                up *= scale.y;
                meshVertices[i] = splinePos + right * v.x + up * v.y;
                meshNormals[i] = right * n.x + up * n.y + forward * n.z;
            }
            var meshUvs = mesh.uv;
            var meshIndices = mesh.triangles.Select(x => vertexOffset + x).ToArray();
            
            Array.Copy(meshVertices, 0, vertices, vertexOffset, vertexCount);
            Array.Copy(meshNormals, 0, normals, vertexOffset, vertexCount);
            Array.Copy(meshUvs, 0, uvs, vertexOffset, vertexCount);
            Array.Copy(meshIndices, 0, indices, indexOffset, meshIndices.Length);

            vertexOffset += vertexCount;
            indexOffset += meshIndices.Length;
            t += (mesh.bounds.size.z + spacing) / sequenceLength;
        }

        return new Mesh
        {
            indexFormat = indices.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16,
            vertices = vertices,
            triangles = indices,
            normals = normals,
            uv = uvs
        };
    }

    public override Mesh GenerateMesh(SplineMeshData spline) => GenerateMesh(spline, false);
    public override Mesh GenerateCollider(SplineMeshData spline) => GenerateMesh(spline, true);

    public override Vector4 Bounds =>
        new(
            meshes.Average(x => x.mesh.bounds.min.x),
            meshes.Average(x => x.mesh.bounds.max.x),
            meshes.Average(x => x.mesh.bounds.min.y),
            meshes.Average(x => x.mesh.bounds.max.y)
        );
}
