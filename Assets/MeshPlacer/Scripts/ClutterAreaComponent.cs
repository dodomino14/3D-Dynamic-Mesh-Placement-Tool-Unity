using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutterAreaComponent : MonoBehaviour
{
    public Vector3 AreaDimensions;
    public Vector3 AreaCenter;
    public float SphereRadius;


    private enum AreaType {Box, Cylinder, Mesh, Polygon}
    [SerializeField] private AreaType areaType;

}
