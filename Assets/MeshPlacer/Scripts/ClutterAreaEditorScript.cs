using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ClutterCube
{
    public Vector3 Top {get => _top; set{ _top = value; CalculateCenter();}}
    public Vector3 Bottom {get => _bottom; set{ _bottom = value; CalculateCenter();}}
    public Vector3 Left {get => _left; set{ _left = value; CalculateCenter();}}
    public Vector3 Right {get => _right; set{ _right = value; CalculateCenter();}}
    public Vector3 Forward {get => _forward; set{ _forward = value; CalculateCenter();}}
    public Vector3 Back {get => _back; set{ _back = value; CalculateCenter();}}
    public Vector3 Center {get => _center; set => _center = value;}
    private Vector3 _top, _bottom, _left, _right, _forward, _back;
    private Vector3 _center;
    private void CalculateCenter()
    {
        Vector3 newCenter = Vector3.zero;

        newCenter.x = (Left.x + Right.x) / 2f;
        newCenter.y = (Top.y + Bottom.y) / 2f;
        newCenter.z = (Forward.z + Back.z) / 2f;
        _center = newCenter;
    }
}

[CustomEditor(typeof(ClutterAreaComponent))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;
    private Vector3 _halfDimensions;
    private enum Direction {Left, Right, Forward, Back, Up, Down}
    private Dictionary<Direction, Vector3> _directionMap = new Dictionary<Direction, Vector3>();
    void OnEnable()
    {
        _directionMap.Add(Direction.Left, Vector3.left); 
        _directionMap.Add(Direction.Right, Vector3.right); 
        _directionMap.Add(Direction.Up, Vector3.up); 
        _directionMap.Add(Direction.Down, Vector3.down); 
        _directionMap.Add(Direction.Forward, Vector3.forward); 
        _directionMap.Add(Direction.Back, Vector3.back); 
    }
    void OnSceneGUI()
    {
        _clutter = target as ClutterAreaComponent;
        _halfDimensions = _clutter.AreaDimensions / 2f;
        DrawBoxedClutterArea();
    }

    private void DrawBoxedClutterArea()
    {
        Handles.DrawWireCube(_clutter.transform.position + _clutter.AreaCenter, _clutter.AreaDimensions);
        
        DrawCornerCircle(Direction.Right);
        DrawCornerCircle(Direction.Left);
        DrawCornerCircle(Direction.Up);
        DrawCornerCircle(Direction.Down);
        DrawCornerCircle(Direction.Forward);
        DrawCornerCircle(Direction.Back);
    }
    private void DrawCornerCircle(Direction direction)
    {
        
        Vector3 cornerPosition = _clutter.AreaCenter + _clutter.transform.position + _directionMap[direction];
        

        EditorGUI.BeginChangeCheck();        
        Vector3 result = Handles.FreeMoveHandle(cornerPosition, _clutter.SphereRadius, Vector3.one * 0.1f, Handles.DotHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            result -= cornerPosition;
            Vector3 newDimensions = _directionMap[direction];
              
            if(newDimensions.x != 0) newDimensions.x = newDimensions.x > 0 ? result.x : -result.x;
            if(newDimensions.y != 0) newDimensions.y = newDimensions.y > 0 ? result.y : -result.y;
            if(newDimensions.z != 0) newDimensions.z = newDimensions.z > 0 ? result.z : -result.z;

            _clutter.AreaDimensions += newDimensions;

        }
    }
}

