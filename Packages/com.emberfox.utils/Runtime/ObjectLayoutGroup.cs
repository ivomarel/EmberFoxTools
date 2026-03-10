using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ObjectLayoutGroup : MonoBehaviour
{
    public bool runOnUpdate;
    public Vector3 positionOffset;
    public float anchor = 0.5f;
    public Vector3 rotation;

    private void Start()
    {
        UpdateLayout();
    }

    private void Update()
    {
        if (!Application.isPlaying || runOnUpdate)
        {
            UpdateLayout();
        }
    }

    void UpdateLayout()
    {
        var activeChildren = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildren.Add(child);
            }
        }
        
        for (int i = 0; i < activeChildren.Count; i++)
        {
            var child = activeChildren[i];
            child.localPosition = -positionOffset * (i - (activeChildren.Count - 1) * anchor);
            child.localEulerAngles = rotation;
        }
    }
}
