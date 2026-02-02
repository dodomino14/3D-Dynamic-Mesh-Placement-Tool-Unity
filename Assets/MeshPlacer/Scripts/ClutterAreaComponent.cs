using System;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
public class ClutterAreaComponent : MonoBehaviour
{
    public Vector3 Dimensions;
    public Vector3 CenterPoint;
    public float SphereRadius;
    private enum AreaType {Box, Cylinder, Mesh, Polygon}
    [SerializeField] private AreaType areaType;

}
