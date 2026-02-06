using System;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(ClutterAreaVisualizer))]
public class ClutterAreaComponent : MonoBehaviour
{
    [DelayedProperty, OnValueChanged("DimensionChanged")]
    public Vector3 Dimensions;
    [OnValueChanged("DimensionChanged")]
    public Vector3 CenterPoint;
    public Vector2 GridSize;

    private enum AreaType {Box, Cylinder, Mesh, Polygon}
    [SerializeField] private AreaType areaType;
    private void DimensionChanged()
    {
        GetComponent<ClutterAreaVisualizer>().DimensionsUpdated();
    }
    [Button]
    public void RebuildVisualization()
    {
        GetComponent<ClutterAreaVisualizer>().Cube = new ClutterCube(gameObject);
        GetComponent<ClutterAreaVisualizer>().DimensionsUpdated();
    }
}
