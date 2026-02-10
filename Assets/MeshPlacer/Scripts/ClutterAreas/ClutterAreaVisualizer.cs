using UnityEngine;
using Sirenix.OdinInspector;
public enum GridDisplayStyle {BaseGrid, VerticalGrid, FullGrid, None}

[RequireComponent(typeof(ClutterAreaComponent))]
public class ClutterAreaVisualizer : SerializedMonoBehaviour
{
    public GridDisplayStyle GridDisplay;
    [SerializeField] public ClutterCube Cube;

    public delegate void DimensionUpdate();
    public event DimensionUpdate OnDimensionsUpdatedInInspector;
    public void DimensionsUpdated()
    {
        OnDimensionsUpdatedInInspector.Invoke();
    }
}
