using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClutterCube
{
    public ClutterSquare TopSquare, DownSquare, FrontSquare, BackSquare, LeftSquare, RightSquare;
    public Vector3 GetCenter()
    {
        return (TopSquare.GetCenter() + FrontSquare.GetCenter() + DownSquare.GetCenter() + 
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
        TopSquare = new ClutterSquare{Top = topFront, Down = topBack, Left = topLeft, Right = topRight};
        DownSquare = new ClutterSquare{Top = downFront, Down = downBack, Left = downLeft, Right = downRight};
        FrontSquare = new ClutterSquare{Top = topFront, Down = downFront, Left = forwardLeft, Right = forwardRight};
        BackSquare = new ClutterSquare{Top = topBack, Down = downBack, Left = backLeft, Right = backRight};
        LeftSquare = new ClutterSquare{Top = topLeft, Down = downLeft, Left = forwardLeft, Right = backLeft};
        RightSquare = new ClutterSquare{Top = topRight, Down = downRight, Left = forwardRight, Right = backRight};

        TopSquare.MovementAxis = Vector3Int.up;
        DownSquare.MovementAxis = Vector3Int.down;
        FrontSquare.MovementAxis = Vector3Int.forward;
        BackSquare.MovementAxis = Vector3Int.back;
        LeftSquare.MovementAxis = Vector3Int.left;
        RightSquare.MovementAxis = Vector3Int.right;
    }
}
public class ClutterSquare
{
    public ClutterLine Top, Down, Left, Right;
    public Vector3Int MovementAxis;
    public void Slide(Vector3 distance)
    {
        Top.Translate(distance);
        Down.Translate(distance);
        Left.Translate(distance);
        Right.Translate(distance);
    }
    public Vector3 GetCenter() => (Top.GetCenter() + Down.GetCenter() + Left.GetCenter() + Right.GetCenter()) / 4f;
}
public class ClutterLine
{
    public ClutterPoint P1, P2;
    public void Translate(Vector3 destination)
    {
        if(destination.x != 0) TranslateX(destination.x);
        if(destination.y != 0) TranslateY(destination.y);
        if(destination.z != 0) TranslateZ(destination.z);
    }
    public void TranslateX(float distance)
    {
        P1.Value.x = distance;
        P2.Value.x = distance;
    }
    public void TranslateY(float distance)
    {
        P1.Value.y = distance;
        P2.Value.y = distance;
    }
    public void TranslateZ(float distance)
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

[CustomEditor(typeof(ClutterAreaComponent))]
public class ClutterAreaEditorScript : Editor
{
    private ClutterAreaComponent _clutter;

    private ClutterCube _cube = new ClutterCube();
    void OnSceneGUI()
    {
        _clutter = target as ClutterAreaComponent;
        DrawBoxedClutterArea();
    }

    private void DrawBoxedClutterArea()
    {
        DrawSquares(_cube.TopSquare);
        DrawSquares(_cube.DownSquare);
        DrawSquares(_cube.FrontSquare);
        DrawSquares(_cube.BackSquare);
        DrawSquares(_cube.LeftSquare);
        DrawSquares(_cube.RightSquare);

    }
    private void DrawSquares(ClutterSquare square)
    {
        Vector3 center = _clutter.transform.position;
        Handles.DrawLine(center + square.Top.P1.Value , center +  square.Top.P2.Value);
        Handles.DrawLine(center + square.Down.P1.Value, center +  square.Down.P2.Value);
        Handles.DrawLine(center + square.Left.P1.Value, center +  square.Left.P2.Value);
        Handles.DrawLine(center + square.Right.P1.Value, center +  square.Right.P2.Value);
        DrawSquareHandle(square);

    }
    private void DrawSquareHandle(ClutterSquare square)
    {
        Vector3 squareCenter = (square.Top.P1.Value + square.Down.P2.Value) / 2f;
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
        }
    }
}

