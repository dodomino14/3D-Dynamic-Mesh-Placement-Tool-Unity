using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[CustomEditor(typeof(ClutterAreaVisualizer))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;
    private ClutterCube _cube;
    private ClutterAreaVisualizer _visualizer;
    private MeshPlacementSettings _settings;
    void OnEnable()
    {
        _visualizer = target as ClutterAreaVisualizer;
        ConfigureDefaults();
        _visualizer.OnDimensionsUpdatedInInspector += DimensionsUpdatedInEditor;
    }
    void ConfigureDefaults()
    {
        _visualizer = target as ClutterAreaVisualizer;
        _clutter = _visualizer.GetComponent<ClutterAreaComponent>();
        _settings = MeshPlacementManager.Settings;
        if(_visualizer.Cube == null) _visualizer.Cube = new ClutterCube(_visualizer.gameObject.transform);
        _cube = _visualizer.Cube;
        _cube.GenerateUnSerializedData();
        UpdateInspectorValues();
        if(_settings != null && _settings.HideGizmosOnEnable) Tools.current = Tool.None;
        else Tools.current = Tool.Move;
    }
    void OnDisable()
    {
        _visualizer.OnDimensionsUpdatedInInspector -= DimensionsUpdatedInEditor;
        EditorUtility.SetDirty(_visualizer);
        Tools.current = Tool.Move;
    }
    void OnSceneGUI()
    {
        _settings = MeshPlacementManager.Settings;
        if(_cube.Parent == null) return;
        DrawCenter();
        DrawBoxedClutterArea();
        DrawBoxDimensions();
        DrawGrid();
    }
    private void DrawGrid()
    {
        if(_clutter.GridSize.x <= 0 || _clutter.GridSize.y <= 0) return;
        GridDisplayStyle displayFormat = _visualizer.GridDisplay;
        Vector3 basePosition = _visualizer.gameObject.transform.position;
        if(displayFormat == GridDisplayStyle.BaseGrid || displayFormat == GridDisplayStyle.FullGrid)
        {
            DrawGridRows(_cube.BottomFrontLeft.Value, _cube.BottomBackRight.Value, basePosition);
            DrawGridColumns(_cube.BottomFrontLeft.Value, _cube.BottomBackRight.Value, basePosition);
        }
        Handles.color = _settings.DefaultColor;
    }
    private void DrawGridColumns(Vector3 frontLeft, Vector3 bottomRight, Vector3 basePosition)
    {
        Handles.color = _settings.GridColor;
        Vector3 areaDimensions = bottomRight - frontLeft;
        Vector3 start = frontLeft;
        Vector3 end = start + new Vector3(0, 0, areaDimensions.z);
        int columns = (int) Mathf.Abs(areaDimensions.x / _clutter.GridSize.y);
        for(int y = 0; y < columns - 1; y++)
        {
            start.x += _clutter.GridSize.x;
            end.x = start.x;
            Handles.DrawLine(_clutter.transform.rotation * start + basePosition, _clutter.transform.rotation * end + basePosition);
        }
        start.x += _clutter.GridSize.x;
        
        if(Mathf.Abs(start.x - bottomRight.x) > 0.01f)
            DrawErrorArea(start, bottomRight);
    }
    private void DrawGridRows(Vector3 frontLeft, Vector3 bottomRight, Vector3 basePosition)
    {
        Handles.color = _settings.GridColor;
        Vector3 areaDimensions = bottomRight - frontLeft;
        Vector3 start = frontLeft;
        Vector3 end = start + new Vector3(areaDimensions.x, 0, 0);
        int rows = (int) Mathf.Abs(areaDimensions.z / _clutter.GridSize.x);
        for(int y = 0; y < rows - 1; y++)
        {
            start.z -= _clutter.GridSize.y;
            end.z = start.z;
            Handles.DrawLine(_clutter.transform.rotation * start + basePosition, _clutter.transform.rotation * end + basePosition);
        }
        start.z -= _clutter.GridSize.y;
        
        if(Mathf.Abs(start.z - bottomRight.z) > 0.01f)
            DrawErrorArea(start, bottomRight);
    }
    private void DrawErrorArea(Vector3 start, Vector3 end)
    {
        //Later draw call will mix whatever color it's given with the Handles color, so it's best for this to always be set to white
        Handles.color = Color.white;
        //Starts top-left, goes clockwise
        Vector3[] verts = {start, new Vector3(end.x, start.y, start.z), end, new Vector3(start.x, start.y, end.z)};
        for(int i = 0; i < verts.Length; i++)
        {
            verts[i] = _clutter.transform.rotation * verts[i] + _clutter.transform.position;
        }
        Handles.DrawSolidRectangleWithOutline(verts, _settings.ErrorColor, Color.darkSlateGray);
    }
    private void DrawCenter()
    {
        Handles.color = _settings.CenterColor;
        EditorGUI.BeginChangeCheck();
        Vector3 currentCenter = _cube.LocalCenter;
        Vector3 handlePosition = Handles.PositionHandle(currentCenter + _clutter.transform.position, Quaternion.identity);
        //Vector3 handlePosition = Handles.FreeMoveHandle(_cube.LocalCenter + _clutter.transform.position, _settings.HandleSize * 2f, _settings.SnapIncrement * Vector3.one, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            handlePosition -= _clutter.transform.position;
            _cube.Slide(handlePosition - _cube.Center);
            UpdateInspectorValues();
        }
        Handles.color = _settings.DefaultColor;
    }
    private void DrawBoxedClutterArea()
    {
        DrawSquare(_cube.Top);
        DrawSquare(_cube.Bottom);
        DrawSquare(_cube.Front);
        DrawSquare(_cube.Back);
        DrawSquare(_cube.Left);
        DrawSquare(_cube.Right);
    }
    private void DrawBoxDimensions()
    {
        DrawDimension(Color.blue,_cube.BottomBackRight.Local, _cube.BottomFrontRight.Local);
        DrawDimension(Color.red, _cube.BottomFrontLeft.Local, _cube.BottomFrontRight.Local);
        DrawDimension(Color.green, _cube.BottomFrontRight.Local, _cube.TopFrontRight.Local);
    }
    private void DrawDimension(Color color, Vector3 pointOne, Vector3 pointTwo)
    {
        Vector3 basePosition = _clutter.transform.position;
        Handles.color = color;
        Handles.DrawLine(basePosition + pointOne, basePosition + pointTwo, 4f);
        Handles.color = _settings.TextColor;
        Handles.Label(basePosition + (pointOne + pointTwo) / 2f, (pointOne - pointTwo).magnitude + "", _settings.TextStyle);
        Handles.color = _settings.DefaultColor;
    }
    private void DrawSquare(List<ClutterPoint> points)
    {
        Vector3 center = _clutter.transform.position;
        for(int i = 1; i < points.Count; i++)
        {
            Handles.DrawLine(center + points[i - 1].Value, center + points[i].Value);
        }
        Handles.DrawLine(center + points[points.Count - 1].Value, center + points[0].Value);
        DrawSquareHandle(points);
    }
    private void DrawSquareHandle(List<ClutterPoint> points)
    {
        Vector3 centerPosition = _clutter.transform.position + ClutterCube.GetCenterPoint(points);
        EditorGUI.BeginChangeCheck();
        Vector3 sliderMovement = Handles.Slider(centerPosition, _cube.GetMovementDirection(points), _settings.HandleSize, Handles.DotHandleCap, _settings.SnapIncrement);
        if (EditorGUI.EndChangeCheck())
        {
            sliderMovement -= _clutter.transform.position;
            _cube.MovePointsAlongAxis(points, sliderMovement);
            UpdateInspectorValues();
        }
    }
    private void UpdateInspectorValues()
    {
        Vector3 dimensions = _cube.Dimensions;
        _clutter.Dimensions.x = dimensions.x;
        _clutter.Dimensions.y = dimensions.y;
        _clutter.Dimensions.z = dimensions.z;
        _clutter.CenterPoint = _cube.Center;
    }
    private void DimensionsUpdatedInEditor()
    {
        Vector3 changedBy = _clutter.Dimensions - _cube.Dimensions;
        Vector3 centerDelta = _clutter.CenterPoint - _cube.Center;
        _cube = _visualizer.Cube;
        if(changedBy.x != 0)
        {
            _cube.SlidePointsAlongAxis(_cube.Right, changedBy.x / 2f);
            _cube.SlidePointsAlongAxis(_cube.Left, -changedBy.x / 2f);
        }
        if(changedBy.y != 0)
        {
            _cube.SlidePointsAlongAxis(_cube.Top, changedBy.y / 2f);
            _cube.SlidePointsAlongAxis(_cube.Bottom, -changedBy.y / 2f);
        }
        if(changedBy.z != 0)
        {
            _cube.SlidePointsAlongAxis(_cube.Front, changedBy.z / 2f);
            _cube.SlidePointsAlongAxis(_cube.Back, -changedBy.z / 2f);
        } 
        _cube.Slide(centerDelta);
        ConfigureDefaults();
    }
}