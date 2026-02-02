using System;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
[RequireComponent(typeof(ClutterAreaVisualizer))]
public class ClutterAreaComponent : MonoBehaviour
{
    [DelayedProperty, OnValueChanged("DimensionChanged")]
    public Vector3 Dimensions;
    public Vector3 CenterPoint;
    public float SphereRadius;
    private enum AreaType {Box, Cylinder, Mesh, Polygon}
    [SerializeField] private AreaType areaType;
    private void DimensionChanged()
    {
        GetComponent<ClutterAreaVisualizer>().DimensionsUpdated();
    }
}
