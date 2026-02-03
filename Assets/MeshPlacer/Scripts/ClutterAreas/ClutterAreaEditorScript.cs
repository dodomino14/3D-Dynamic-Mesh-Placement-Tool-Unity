using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ClutterAreaVisualizer))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;
    private ClutterCube _cube;
    private ClutterAreaVisualizer _visualizer;
    private Vector3 _previousDimensions;
    private Vector3 _previousCenter;
    void OnEnable()
    {
        _visualizer = target as ClutterAreaVisualizer;
        _clutter = _visualizer.GetComponent<ClutterAreaComponent>();
        _cube = _visualizer.Cube;
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
        DrawCenter();
        DrawGrid();
    }
    private void DrawGrid()
    {
        if(_clutter.GridSize.x <= 0 || _clutter.GridSize.y <= 0) return;
        Handles.color = Color.green;
        if(_clutter.GridDisplay == GridDisplayStyle.BaseGrid || _clutter.GridDisplay == GridDisplayStyle.FullGrid)
        {
            Vector3 bottomFrontLeft = _clutter.transform.position +  _cube.ForwardSquare.Bottom.P1.Value;
            Vector3 bottomBackRight = _clutter.transform.position + _cube.BackSquare.Bottom.P2.Value;
            DrawGridRows(bottomFrontLeft, bottomBackRight);
            DrawGridColumns(bottomFrontLeft, bottomBackRight);
        }

        if(_clutter.GridDisplay == GridDisplayStyle.VerticalGrid || _clutter.GridDisplay == GridDisplayStyle.FullGrid)
        {
            Vector3 bottomBackLeft = _cube.BackSquare.Bottom.P1.Value;
            Vector3 topBackRight = _cube.TopSquare.Bottom.P2.Value;
            DrawGridRows(bottomBackLeft, topBackRight);
            DrawGridColumns(bottomBackLeft, topBackRight);
        }

        Handles.color = Color.white;
    }
    private void DrawGridColumns(Vector3 frontLeft, Vector3 bottomRight)
    {
        Vector3 areaDimensions = bottomRight - frontLeft;
        int columns = (int) Mathf.Abs(areaDimensions.x / _clutter.GridSize.y);
        Vector3 start = frontLeft;
        Vector3 end = start + new Vector3(0, 0, areaDimensions.z);
        for(int y = 0; y < columns - 1; y++)
        {
            start.x += _clutter.GridSize.x;
            end.x = start.x;
            Handles.DrawLine(start, end);
        }
        start.x += _clutter.GridSize.x;
        
        if(Mathf.Abs(start.x - bottomRight.x) > 0.01f)
            DrawErrorArea(start, bottomRight);
    }
    private void DrawGridRows(Vector3 frontLeft, Vector3 bottomRight)
    {
        Vector3 areaDimensions = bottomRight - frontLeft;
        Vector3 start = frontLeft;
        Vector3 end = start + new Vector3(areaDimensions.x, 0, 0);
        int rows = (int) Mathf.Abs(areaDimensions.z / _clutter.GridSize.x);
        for(int y = 0; y < rows - 1; y++)
        {
            start.z -= _clutter.GridSize.y;
            end.z = start.z;
            Handles.DrawLine(start, end);
        }
        start.z -= _clutter.GridSize.y;
        
        if(Mathf.Abs(start.z - bottomRight.z) > 0.01f)
            DrawErrorArea(start, bottomRight);
    }
    private void DrawErrorArea(Vector3 start, Vector3 end)
    {
        Handles.color = Color.white;
        //Starts top-left, goes clockwise
        Vector3[] verts = {start, new Vector3(end.x, start.y, start.z), end, new Vector3(start.x, start.y, end.z)};
        Handles.DrawSolidRectangleWithOutline(verts, new Color(1, 1, 0, 0.5f), Color.darkSlateGray);
    }
    private void DrawCenter()
    {
        Handles.color = Color.pink;
        Handles.DrawSolidDisc(_cube.Center + _clutter.transform.position, Vector3.up, 0.05f);
        Handles.color = Color.white;
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