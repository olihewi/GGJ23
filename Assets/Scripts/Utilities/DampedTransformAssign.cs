using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DampedTransformAssign : MonoBehaviour
{
    [ContextMenu("Apply Damped Transform Parents")]
    private void ApplyDampedTransformParents()
    {
        foreach (var dampedTransform in GetComponentsInChildren<DampedTransform>())
        {
            dampedTransform.data.constrainedObject = dampedTransform.transform;
            dampedTransform.data.sourceObject = dampedTransform.transform.parent;
            dampedTransform.data.dampPosition = 0.25F;
            dampedTransform.data.dampRotation = 0.7F;
        }
    }
}
