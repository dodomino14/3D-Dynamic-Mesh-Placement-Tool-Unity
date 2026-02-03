using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ClutterAreaVisualizer))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;
    ClutterAreaVisualizer _visualizer;
    private ClutterCube _cube = new ClutterCube();
    private Vector3 _previousDimensions;
    private Vector3 _previousCenter;
    void OnEnable()
    {
        _visualizer = target as ClutterAreaVisualizer;
        _clutter = _visualizer.GetComponent<ClutterAreaComponent>();
        _visualizer.OnDimensionsUpdatedInInspector += DimensionsUpdatedInEditor;
        _previousDimensions = _clutter.Dimensions;
        _previousCenter = _cube.Center;
        UpdateInspectorValues();
    }
    void OnDisable()
    {
        _visualizer.OnDimensionsUpdatedInInspector -= DimensionsUpdatedInEditor;
    }
    void OnSceneGUI()
    {
        DrawBoxedClutterArea();
        DrawBoxDimensions();
    }
    private void DrawBoxedClutterArea()
    {
        DrawSquare(_cube.TopSquare);
        DrawSquare(_cube.BottomSquare);
        DrawSquare(_cube.ForwardSquare);
        DrawSquare(_cube.BackSquare);
        DrawSquare(_cube.LeftSquare);
        DrawSquare(_cube.RightSquare);
    }
    private void DrawBoxDimensions()
    {
        DrawDimension(Color.blue, _cube.BottomForwardRight.Value, _cube.BottomBottomRight.Value);
        DrawDimension(Color.red, _cube.BottomBottomLeft.Value, _cube.BottomBottomRight.Value);
        DrawDimension(Color.green, _cube.BottomBottomRight.Value, _cube.TopBottomRight.Value);
    }
    private void DrawDimension(Color color, Vector3 pointOne, Vector3 pointTwo)
    {
        Vector3 basePosition = _clutter.transform.position;
        Color baseColor = Handles.color;
        Handles.color = color;
        Handles.DrawLine(basePosition + pointOne, basePosition + pointTwo, 4f);
        Handles.color = Color.white;
        Handles.Label(basePosition + (pointOne + pointTwo) / 2f, (pointOne - pointTwo).magnitude + "");
        Handles.color = baseColor;
    }
    private void DrawSquare(ClutterSquare square)
    {
        Vector3 center = _clutter.transform.position ;
        Handles.DrawLine(center + square.Top.P1.Value , center + square.Top.P2.Value);
        Handles.DrawLine(center + square.Bottom.P1.Value, center + square.Bottom.P2.Value);
        Handles.DrawLine(center + square.Left.P1.Value, center + square.Left.P2.Value);
        Handles.DrawLine(center + square.Right.P1.Value, center + square.Right.P2.Value);
        DrawSquareHandle(square);
    }
    private void DrawSquareHandle(ClutterSquare square)
    {
        Vector3 centerPosition = _clutter.transform.position + square.Center;
        EditorGUI.BeginChangeCheck();        
        Vector3 sliderMovement = Handles.Slider(centerPosition, square.MovementAxis, 0.025f, Handles.DotHandleCap, 0.1f);
        if (EditorGUI.EndChangeCheck())
        {
            sliderMovement -= _clutter.transform.position;
            LockMovementAxis(square, ref sliderMovement);
            square.Slide(sliderMovement);
            UpdateInspectorValues();
        }
    }
    private void LockMovementAxis(ClutterSquare square, ref Vector3 movement)
    {
        if(square.MovementAxis.x == 0) movement.x = 0;
        if(square.MovementAxis.y == 0) movement.y = 0;
        if(square.MovementAxis.z == 0) movement.z = 0;
    }
    private void UpdateInspectorValues()
    {
        _clutter.Dimensions.x = _cube.Width;
        _clutter.Dimensions.y = _cube.Height;
        _clutter.Dimensions.z = _cube.Depth;
        _clutter.CenterPoint = _cube.Center;
        _previousDimensions = _clutter.Dimensions;
        _previousCenter = _cube.GetCenter();
    }
    private void DimensionsUpdatedInEditor()
    {
        Vector3 changedBy = _clutter.Dimensions - _previousDimensions;
        Vector3 centerDelta = _clutter.CenterPoint - _previousCenter;
        if(changedBy.x != 0)
            _cube.TranslateAxis(_cube.HorizontalAxis, changedBy);
        if(changedBy.y != 0)
            _cube.TranslateAxis(_cube.VerticalAxis, changedBy);
        if(changedBy.z != 0)
            _cube.TranslateAxis(_cube.DepthAxis, changedBy);

        _cube.Translate(centerDelta);
        _previousCenter = _cube.GetCenter();
        _previousDimensions = _clutter.Dimensions;
    }
}