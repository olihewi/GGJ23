using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[CustomEditor(typeof(SplineMesh)), CanEditMultipleObjects]
public class SplineMeshEditor : Editor
{
    private List<float> editingKnots = new();
    private List<int> editingVertices = new();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate!", GUILayout.Height(50)))
        {
            foreach (var t in targets)
            {
                (t as SplineMesh).GenerateMesh();
            }
        }
    }

    private void OnSceneGUI()
    {
        var splineMesh = target as SplineMesh;

        var generator = splineMesh.generator;
        if (generator != null && generator is SplineShapeGenerator shapeGenerator)
        {
            ShapeGeneratorGUI(splineMesh.splineData, shapeGenerator);
        }
        KnotScaleGUI(splineMesh.splineData, generator);
        
        Handles.BeginGUI();
        Vector2 buttonSize = new Vector2(100, 50);
        Rect button = new Rect(Screen.safeArea.max - buttonSize - new Vector2(5, buttonSize.y), buttonSize);
        if (GUI.Button(button, "Generate"))
        {
            splineMesh.GenerateMesh();
        }
        Handles.EndGUI();
    }

    private Vector2[] scales;
    private void KnotScaleGUI(SplineMeshData spline, SplineMeshGenerator generator)
    {
        if (generator == null || spline.spline == null) return;
        var e = Event.current;
        if (scales == null || (e.type == EventType.MouseUp && e.button == 0))
        {
            scales = new Vector2[spline.knotScales.Length];
            Array.Copy(spline.knotScales, scales, scales.Length);
        }
        for (int i = 0; i < Math.Min(spline.knotScales.Length, spline.Knots.Count()); i++)
        {
            var knot = spline.Knots.ElementAt(i);
            var pos = spline.spline.transform.TransformPoint(knot.Position);
            var rot = spline.spline.transform.rotation * knot.Rotation;
            var forward = (rot * knot.TangentOut).normalized;
            var up = rot * Vector3.up;
            var right = Quaternion.AngleAxis(90.0F, up) * forward;
            var bounds = generator.Bounds;

            var xDir = Mathf.Abs(bounds.x) > Mathf.Abs(bounds.y) ? bounds.x : bounds.y;
            var yDir = Mathf.Abs(bounds.z) > Mathf.Abs(bounds.w) ? bounds.z : bounds.w;
            spline.knotScales[i].x = Handles.ScaleSlider(spline.knotScales[i].x, pos, right, rot, xDir * scales[i].x, 0.1F);
            spline.knotScales[i].y = Handles.ScaleSlider(spline.knotScales[i].y, pos, up, rot, yDir * scales[i].y, 0.1F);
        }
    }

    private void ShapeGeneratorGUI(SplineMeshData spline, SplineShapeGenerator generator)
    {
        var serializedGenerator = new SerializedObject(generator);
        var shapeProperty = serializedGenerator.FindProperty("shape");
        var originWS = spline.EvaluatePositionWS(0);
        var upWS = spline.EvaluateUpWS(0);
        var forwardWS = spline.EvaluateForwardWS(0);
        var rightWS = spline.EvaluateRight(upWS, forwardWS);
        Event e = Event.current;


        for (int i = 0; i < generator.shape.Length; i++)
        {
            var vertex = generator.shape[i];
            Vector3 pos = originWS + rightWS * vertex.x + upWS * vertex.y;
            
            if (i + 1 < generator.shape.Length)
            {
                var vertex2 = generator.shape[i+1];
                Vector3 pos2 = originWS + rightWS * vertex2.x + upWS * vertex2.y;
                Handles.color = Color.yellow;
                Handles.DrawLine(pos, pos2);
            }

            if (!editingVertices.Contains(i))
            {
                Vector2 posSS = HandleUtility.WorldToGUIPoint(pos);
                Vector2 mouseSS = e.mousePosition;
                float handleSize = HandleUtility.GetHandleSize(pos) / 20.0F;
                bool hovering = Vector2.Distance(posSS, mouseSS) < 7.5F;
                
                Handles.color = hovering ? Color.white : Color.yellow;
                Handles.DotHandleCap(GUIUtility.GetControlID(FocusType.Passive), pos, Quaternion.identity, handleSize, EventType.Layout);
                Handles.DotHandleCap(GUIUtility.GetControlID(FocusType.Passive), pos, Quaternion.identity, handleSize, EventType.Repaint);
                
                if (Vector2.Distance(posSS, mouseSS) < 7.5F)
                {
                    if (e.shift)
                        EditorGUIUtility.AddCursorRect(Screen.safeArea, MouseCursor.ArrowPlus);
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        if (!e.shift) editingVertices.Clear();
                        editingVertices.Add(i);
                    }
                }
            }
        }

        for (int i = 0; i < editingVertices.Count; i++)
        {
            var vertex = generator.shape[editingVertices[i]];
            Vector3 pos = originWS + rightWS * vertex.x + upWS * vertex.y;
            Vector3 handlePos = Handles.DoPositionHandle(pos, Quaternion.LookRotation(forwardWS, upWS));
            if (handlePos != pos)
            {
                var diff = handlePos - pos;
                foreach (int j in editingVertices)
                {
                    var vertexProperty = shapeProperty.GetArrayElementAtIndex(j);
                    var value = spline.spline.transform.InverseTransformDirection(diff);
                    value.z = 0.0F;
                    vertexProperty.vector3Value += value;
                }
                serializedGenerator.ApplyModifiedProperties();
                (target as SplineMesh).GenerateMesh();
            }
        }
        
        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Delete)
            {
                for (int i = 0; i < editingVertices.Count; i++)
                {
                    shapeProperty.DeleteArrayElementAtIndex(editingVertices[i]);
                    for (int j = 0; j < editingVertices.Count; j++)
                    {
                        if (editingVertices[j] > editingVertices[i]) editingVertices[j]--;
                    }
                    editingVertices.RemoveAt(i--);
                }
                serializedGenerator.ApplyModifiedProperties();
                (target as SplineMesh).GenerateMesh();
                GUIUtility.hotControl = 0;
                e.Use();
            }
            else if (e.keyCode == KeyCode.D && e.shift)
            {
                for (int i = 0; i < editingVertices.Count; i++)
                {
                    var j = shapeProperty.arraySize;
                    shapeProperty.InsertArrayElementAtIndex(j);
                    shapeProperty.GetArrayElementAtIndex(j).vector3Value = generator.shape[editingVertices[i]];
                    editingVertices[i] = j;
                }
                serializedGenerator.ApplyModifiedProperties();
                GUIUtility.hotControl = 0;
                e.Use();
            }
        }
    }
}
