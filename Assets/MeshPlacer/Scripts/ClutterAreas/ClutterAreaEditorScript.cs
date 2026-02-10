using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.InputSystem;

[CustomEditor(typeof(ClutterAreaVisualizer))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;
    [SerializeField] private ClutterCube _cube;
    private ClutterAreaVisualizer _visualizer;
    private Vector3 _previousDimensions;
    private Vector3 _previousCenter;
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
        if(_cube == null)
        {
            if(_visualizer.Cube == null) 
                _visualizer.Cube = new ClutterCube(_visualizer.gameObject);
            _cube = _visualizer.Cube;
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(_visualizer);
        _previousDimensions = _clutter.Dimensions;
        _previousCenter = _cube.Center;
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
        if(_cube.GameObject == null) return;
        DrawCenter();
        DrawBoxedClutterArea();
        DrawBoxDimensions();
        DrawGrid();
    }
    private void DrawGrid()
    {
        if(_clutter.GridSize.x <= 0 || _clutter.GridSize.y <= 0) return;
        Handles.color = _settings.GridColor;
        GridDisplayStyle displayFormat = _visualizer.GridDisplay;
        if(displayFormat == GridDisplayStyle.BaseGrid || displayFormat == GridDisplayStyle.FullGrid)
        {
            DrawGridRows(_cube.ForwardSquare.Bottom.P1.Value, _cube.BackSquare.Bottom.P2.Value, _visualizer.gameObject.transform.position);
            DrawGridColumns(_cube.ForwardSquare.Bottom.P1.Value, _cube.BackSquare.Bottom.P2.Value, _visualizer.gameObject.transform.position);
        }

        if(displayFormat == GridDisplayStyle.VerticalGrid || displayFormat == GridDisplayStyle.FullGrid)
        {
            DrawGridRows(_cube.BackSquare.Bottom.P1.Value, _cube.TopSquare.Bottom.P2.Value, _visualizer.gameObject.transform.position);
            DrawGridColumns(_cube.BackSquare.Bottom.P1.Value, _cube.TopSquare.Bottom.P2.Value, _visualizer.gameObject.transform.position);
        }

        Handles.color = _settings.DefaultColor;
    }
    private void DrawGridColumns(Vector3 frontLeft, Vector3 bottomRight, Vector3 basePosition)
    {
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
        Vector3 handlePosition = Handles.FreeMoveHandle(_cube.LocalCenter + _clutter.transform.position, _settings.HandleSize * 2f, _settings.SnapIncrement * Vector3.one, Handles.SphereHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            if(_settings.XKey.IsPressed()) handlePosition = new Vector3(handlePosition.x - _cube.GameObject.transform.position.x, currentCenter.y, currentCenter.z);
            else if(_settings.YKey.IsPressed()) handlePosition = new Vector3(currentCenter.x, handlePosition.y - _cube.GameObject.transform.position.y, currentCenter.z);
            else if(_settings.ZKey.IsPressed()) handlePosition = new Vector3(currentCenter.x, currentCenter.y, handlePosition.z- _cube.GameObject.transform.position.z);
            else handlePosition = currentCenter;
            
            _cube.MoveTo(handlePosition);
            UpdateInspectorValues();
        }
        Handles.color = _settings.DefaultColor;

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
        DrawDimension(Color.blue, _cube.BottomForwardRight.LocalValue, _cube.BottomBottomRight.LocalValue);
        DrawDimension(Color.red, _cube.BottomBottomLeft.LocalValue, _cube.BottomBottomRight.LocalValue);
        DrawDimension(Color.green, _cube.BottomBottomRight.LocalValue, _cube.TopBottomRight.LocalValue);
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
    private void DrawSquare(ClutterSquare square)
    {
        Vector3 center = _clutter.transform.position;
        Handles.DrawLine(center + square.Top.P1.LocalValue, center + square.Top.P2.LocalValue);
        Handles.DrawLine(center + square.Bottom.P1.LocalValue, center + square.Bottom.P2.LocalValue);
        Handles.DrawLine(center + square.Left.P1.LocalValue, center + square.Left.P2.LocalValue);
        Handles.DrawLine(center + square.Right.P1.LocalValue, center + square.Right.P2.LocalValue);
        DrawSquareHandle(square);
    }
    private void DrawSquareHandle(ClutterSquare square)
    {
        Vector3 centerPosition = _clutter.transform.position + square.LocalCenter;
        EditorGUI.BeginChangeCheck();        
        Vector3 sliderMovement = Handles.Slider(centerPosition, square.MovementAxis, _settings.HandleSize, Handles.DotHandleCap, _settings.SnapIncrement);
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
        _previousDimensions = _clutter.Dimensions;
        _clutter.CenterPoint = _cube.Center;
        _previousCenter = _cube.Center;
    }
    private void DimensionsUpdatedInEditor()
    {
        Vector3 changedBy = _clutter.Dimensions - _previousDimensions;
        Vector3 centerDelta = _clutter.CenterPoint - _previousCenter;
        _cube = _visualizer.Cube;
        if(changedBy.x != 0)
            _cube.TranslateAxis(_cube.HorizontalAxis, changedBy);
        if(changedBy.y != 0)
            _cube.TranslateAxis(_cube.VerticalAxis, changedBy);
        if(changedBy.z != 0)
            _cube.TranslateAxis(_cube.DepthAxis, changedBy);

        _cube.Translate(centerDelta);
        ConfigureDefaults();
    }
}