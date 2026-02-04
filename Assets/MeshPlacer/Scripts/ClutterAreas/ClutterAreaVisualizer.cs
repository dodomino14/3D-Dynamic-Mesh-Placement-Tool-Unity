using UnityEngine;
using Sirenix.OdinInspector;
public enum GridDisplayStyle {BaseGrid, VerticalGrid, FullGrid, None}

[RequireComponent(typeof(ClutterAreaComponent))]
public class ClutterAreaVisualizer : SerializedMonoBehaviour
{
    public GridDisplayStyle GridDisplay;

    public delegate void DimensionUpdate();
    public event DimensionUpdate OnDimensionsUpdatedInInspector;
    public ClutterCube Cube = new ClutterCube();

    public void DimensionsUpdated()
    {
        OnDimensionsUpdatedInInspector.Invoke();
    }
}
