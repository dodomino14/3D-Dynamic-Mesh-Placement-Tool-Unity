using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutterAreaComponent : MonoBehaviour
{
    [HideInInspector] public Vector3 AreaDimensions;
    public Vector3 AreaCenter;
    public float SphereRadius;


    private enum AreaType {Box, Cylinder, Mesh, Polygon}
    [SerializeField] private AreaType areaType;
    private Vector3 _areaHalfDimensions;

    void OnDrawGizmos()
    {
        _areaHalfDimensions = AreaDimensions / 2f;


        Event current = Event.current;
    }


}
