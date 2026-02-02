using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ClutterCube
{
    public ClutterSquare TopSquare, BottomSquare, ForwardSquare, BackSquare, LeftSquare, RightSquare;
    public float Width {get => RightSquare.GetCenter().x - LeftSquare.GetCenter().x;}
    public float Height {get => TopSquare.GetCenter().y - BottomSquare.GetCenter().y;}
    public float Depth {get => ForwardSquare.GetCenter().z - BackSquare.GetCenter().z;}
    public (ClutterSquare, ClutterSquare) HorizontalAxis, VerticalAxis, DepthAxis;
    public void TranslateAxis((ClutterSquare, ClutterSquare) axis, Vector3 delta)
    {
        axis.Item1.Translate(delta / 2f);
        axis.Item2.Translate(-delta / 2f);
    }
    public ClutterPoint TopForwardLeft {get => TopSquare.Top.P1;}
    public ClutterPoint TopForwardRight {get => TopSquare.Top.P2;}
    public ClutterPoint TopBottomRight {get => TopSquare.Bottom.P2;}
    public ClutterPoint TopBottomLeft {get => TopSquare.Bottom.P1;}
    public ClutterPoint BottomForwardLeft {get => BottomSquare.Top.P1;}
    public ClutterPoint BottomForwardRight {get => BottomSquare.Top.P2;}
    public ClutterPoint BottomBottomRight {get => BottomSquare.Bottom.P2;}
    public ClutterPoint BottomBottomLeft {get => BottomSquare.Bottom.P1;}
    public Vector3 GetCenter()
    {
        return (TopSquare.GetCenter() + ForwardSquare.GetCenter() + BottomSquare.GetCenter() + 
        BackSquare.GetCenter() + LeftSquare.GetCenter() + RightSquare.GetCenter()) / 6f;
    }
    public ClutterCube()
    {
        //Schema -
        //t/d - top/down
        //f/b - forward/back
        //l/r - left/right
        ClutterPoint tfl = new ClutterPoint{Value = new Vector3(-0.5f, 0.5f, 0.5f)};
        ClutterPoint tfr = new ClutterPoint{Value = new Vector3(0.5f, 0.5f, 0.5f)};
        ClutterPoint tbl = new ClutterPoint{Value = new Vector3(-0.5f, 0.5f, -0.5f)};
        ClutterPoint tbr = new ClutterPoint{Value = new Vector3(0.5f, 0.5f, -0.5f)};

        ClutterPoint dfl = new ClutterPoint{Value = new Vector3(-0.5f, -0.5f, 0.5f)};
        ClutterPoint dfr = new ClutterPoint{Value = new Vector3(0.5f, -0.5f, 0.5f)};
        ClutterPoint dbl = new ClutterPoint{Value = new Vector3(-0.5f, -0.5f, -0.5f)};
        ClutterPoint dbr = new ClutterPoint{Value = new Vector3(0.5f, -0.5f, -0.5f)};
        
        
        ClutterLine topFront = new ClutterLine{P1 = tfl, P2 = tfr};
        ClutterLine topBack =  new ClutterLine{P1 = tbl, P2 = tbr};
        ClutterLine topLeft =  new ClutterLine{P1 = tfl, P2 = tbl};
        ClutterLine topRight =  new ClutterLine{P1 = tfr, P2 = tbr};

        ClutterLine downFront = new ClutterLine{P1 = dfl, P2 = dfr};
        ClutterLine downBack =  new ClutterLine{P1 = dbl, P2 = dbr};
        ClutterLine downLeft =  new ClutterLine{P1 = dfl, P2 = dbl};
        ClutterLine downRight =  new ClutterLine{P1 = dfr, P2 = dbr};

        ClutterLine forwardLeft = new ClutterLine{P1 = tfl, P2 = dfl};
        ClutterLine forwardRight = new ClutterLine{P1 = tfr, P2 = dfr};
        ClutterLine backLeft = new ClutterLine{P1 = tbl, P2 = dbl};
        ClutterLine backRight = new ClutterLine{P1 = tbr, P2 = dbr};
        TopSquare = new ClutterSquare{Top = topFront, Bottom = topBack, Left = topLeft, Right = topRight};
        BottomSquare = new ClutterSquare{Top = downFront, Bottom = downBack, Left = downLeft, Right = downRight};
        ForwardSquare = new ClutterSquare{Top = topFront, Bottom = downFront, Left = forwardLeft, Right = forwardRight};
        BackSquare = new ClutterSquare{Top = topBack, Bottom = downBack, Left = backLeft, Right = backRight};
        LeftSquare = new ClutterSquare{Top = topLeft, Bottom = downLeft, Left = forwardLeft, Right = backLeft};
        RightSquare = new ClutterSquare{Top = topRight, Bottom = downRight, Left = forwardRight, Right = backRight};

        TopSquare.MovementAxis = Vector3Int.up;
        BottomSquare.MovementAxis = Vector3Int.down;
        ForwardSquare.MovementAxis = Vector3Int.forward;
        BackSquare.MovementAxis = Vector3Int.back;
        LeftSquare.MovementAxis = Vector3Int.left;
        RightSquare.MovementAxis = Vector3Int.right;

        HorizontalAxis.Item1 = RightSquare;
        HorizontalAxis.Item2 = LeftSquare;
        
        VerticalAxis.Item1 = TopSquare;
        VerticalAxis.Item2 = BottomSquare;

        DepthAxis.Item1 = ForwardSquare;
        DepthAxis.Item2 = BackSquare;
    }
}
public class ClutterSquare
{
    public ClutterLine Top, Bottom, Left, Right;
    public Vector3Int MovementAxis;
    public void Slide(Vector3 destination)
    {
        Top.Slide(destination);
        Bottom.Slide(destination);
        Left.Slide(destination);
        Right.Slide(destination);
    }
    public void Translate(Vector3 delta)
    {
        //Lines share points, so editing only Top & Bottom will be enough to move all 4 points of a square
        Top.Translate(delta);
        Bottom.Translate(delta);
    }
    public Vector3 GetCenter() => (Top.GetCenter() + Bottom.GetCenter() + Left.GetCenter() + Right.GetCenter()) / 4f;
}
public class ClutterLine
{
    public ClutterPoint P1, P2;
    public void Slide(Vector3 newCenter)
    {
        if(newCenter.x != 0) SetX(newCenter.x);
        if(newCenter.y != 0) SetY(newCenter.y);
        if(newCenter.z != 0) SetZ(newCenter.z);
    }
    public void Translate(Vector3 distance)
    {
        P1.Value += distance;
        P2.Value += distance;
    }
    public void SetX(float destination)
    {
        P1.Value.x = destination;
        P2.Value.x = destination;
    }
    public void SetY(float distance)
    {
        P1.Value.y = distance;
        P2.Value.y = distance;
    }
    public void SetZ(float distance)
    {
        P1.Value.z = distance;
        P2.Value.z = distance;
    }
    public Vector3 GetCenter() => (P1.Value + P2.Value) / 2f;
}
public class ClutterPoint
{
    public Vector3 Value;
}

[CustomEditor(typeof(ClutterAreaVisualizer))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;
    ClutterAreaVisualizer _visualizer;
    private ClutterCube _cube = new ClutterCube();
    private Vector3 _previousDimensions;
    void OnEnable()
    {
        _visualizer = target as ClutterAreaVisualizer;
        _clutter = _visualizer.GetComponent<ClutterAreaComponent>();
        _visualizer.OnDimensionsUpdatedInInspector += DimensionsUpdatedInEditor;
        _previousDimensions = _clutter.Dimensions;
        UpdateDimensionsInInspector();
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
        Vector3 center = _clutter.transform.position;
        Handles.DrawLine(center + square.Top.P1.Value , center +  square.Top.P2.Value);
        Handles.DrawLine(center + square.Bottom.P1.Value, center +  square.Bottom.P2.Value);
        Handles.DrawLine(center + square.Left.P1.Value, center +  square.Left.P2.Value);
        Handles.DrawLine(center + square.Right.P1.Value, center +  square.Right.P2.Value);
        DrawSquareHandle(square);

    }
    private void DrawSquareHandle(ClutterSquare square)
    {
        Vector3 squareCenter = square.GetCenter();
        Vector3 centerPosition = _clutter.transform.position + squareCenter;
        EditorGUI.BeginChangeCheck();        
        Vector3 sliderMovement = Handles.Slider(centerPosition, square.MovementAxis, 0.025f, Handles.DotHandleCap, 0.1f);
        if (EditorGUI.EndChangeCheck())
        {
            sliderMovement -= _clutter.transform.position;
            if(square.MovementAxis.x == 0) sliderMovement.x = 0;
            if(square.MovementAxis.y == 0) sliderMovement.y = 0;
            if(square.MovementAxis.z == 0) sliderMovement.z = 0;
            square.Slide(sliderMovement);
            UpdateDimensionsInInspector();
        }
    }
    private void UpdateDimensionsInInspector()
    {
        _clutter.Dimensions.x = _cube.Width;
        _clutter.Dimensions.y = _cube.Height;
        _clutter.Dimensions.z = _cube.Depth;
        _previousDimensions = _clutter.Dimensions;
    }
    private void DimensionsUpdatedInEditor()
    {
        Vector3 changedBy = _clutter.Dimensions - _previousDimensions;
        if(changedBy.x != 0)
            _cube.TranslateAxis(_cube.HorizontalAxis, changedBy);
        if(changedBy.y != 0)
            _cube.TranslateAxis(_cube.VerticalAxis, changedBy);
        if(changedBy.z != 0)
            _cube.TranslateAxis(_cube.DepthAxis, changedBy);
        _previousDimensions = _clutter.Dimensions;
    }
}