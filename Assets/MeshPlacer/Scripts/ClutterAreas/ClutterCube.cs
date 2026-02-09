using UnityEngine;

[System.Serializable]
public class ClutterCube
{
    public ClutterSquare TopSquare, BottomSquare, ForwardSquare, BackSquare, LeftSquare, RightSquare;
    public Vector3 Center { get => GetCenter();}
    public Vector3 LocalCenter {get => GameObject.transform.rotation * Center;}
    public float Width {get => RightSquare.Center.x - LeftSquare.Center.x;}
    public float Height {get => TopSquare.Center.y - BottomSquare.Center.y;}
    public float Depth {get => ForwardSquare.Center.z - BackSquare.Center.z;}
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

    public GameObject GameObject;
    public Vector3 GetCenter()
    {
        return (TopSquare.Center + BottomSquare.Center + LeftSquare.Center + RightSquare.Center + ForwardSquare.Center + BackSquare.Center) / 6.0f;
    }
    public void Translate(Vector3 newValue)
    {
        TopSquare.Translate(newValue);
        BottomSquare.Translate(newValue);
    }
    public void MoveTo(Vector3 destination)
    {
        Vector3 delta = destination - GetCenter();
        Translate(delta);
    }
    public ClutterCube(GameObject owner)
    {
        if(owner == null) Debug.LogError("Attempted to build clutter cube with a null owner!");
        GameObject = owner;
        //Schema -
        //t/d - top/down
        //f/b - forward/back
        //l/r - left/right
        ClutterPoint tfl = new ClutterPoint(new Vector3(-0.5f, 0.5f, 0.5f), owner);
        ClutterPoint tfr = new ClutterPoint(new Vector3(0.5f, 0.5f, 0.5f), owner);
        ClutterPoint tbl = new ClutterPoint(new Vector3(-0.5f, 0.5f, -0.5f), owner);
        ClutterPoint tbr = new ClutterPoint(new Vector3(0.5f, 0.5f, -0.5f), owner);

        ClutterPoint dfl = new ClutterPoint(new Vector3(-0.5f, -0.5f, 0.5f), owner);
        ClutterPoint dfr = new ClutterPoint(new Vector3(0.5f, -0.5f, 0.5f), owner);
        ClutterPoint dbl = new ClutterPoint(new Vector3(-0.5f, -0.5f, -0.5f), owner);
        ClutterPoint dbr = new ClutterPoint(new Vector3(0.5f, -0.5f, -0.5f), owner);
        
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
        TopSquare = new ClutterSquare (topFront, topBack, topLeft, topRight, owner);
        BottomSquare = new ClutterSquare (downFront, downBack, downLeft, downRight, owner);
        ForwardSquare = new ClutterSquare (topFront, downFront, forwardLeft, forwardRight, owner);
        BackSquare = new ClutterSquare (topBack, downBack, backLeft, backRight, owner);
        LeftSquare = new ClutterSquare (topLeft,downLeft, forwardLeft, backLeft, owner);
        RightSquare = new ClutterSquare (topRight, downRight, forwardRight, backRight, owner);

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
[System.Serializable]
public class ClutterSquare
{
    public ClutterLine Top, Bottom, Left, Right;
    public Vector3Int MovementAxis;
    private GameObject _owner;
    public Vector3 Center {get => GetCenter();}
    public Vector3 LocalCenter {get => _owner.transform.rotation * Center;}
    public ClutterSquare(ClutterLine top, ClutterLine bottom, ClutterLine left, ClutterLine right, GameObject owner)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
        _owner = owner;
    } 
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
    private Vector3 GetCenter()
    {
        return (Top.GetCenter() + Bottom.GetCenter() + Left.GetCenter() + Right.GetCenter()) / 4f;
    }
}
[System.Serializable]
public class ClutterLine
{
    public ClutterPoint P1, P2;

    public void Slide(Vector3 newCenter)
    {
        if(!Mathf.Approximately(newCenter.x, 0)) SetX(newCenter.x);
        if(!Mathf.Approximately(newCenter.y, 0)) SetY(newCenter.y);
        if(!Mathf.Approximately(newCenter.z, 0)) SetZ(newCenter.z);
    }
    public void SetCenter(Vector3 newCenter)
    {
        P1.Value = newCenter;
        P2.Value = newCenter;
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
[System.Serializable]
public class ClutterPoint
{
    public Vector3 Value;
    protected GameObject _owner;
    public Vector3 LocalValue {get => _owner.transform.rotation * Value;}
    public ClutterPoint(Vector3 value, GameObject owner)
    {
        Value = value;
        _owner = owner;
    }

}