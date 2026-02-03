using UnityEngine;
using Sirenix.OdinInspector;
[RequireComponent(typeof(ClutterAreaComponent))]
public class ClutterAreaVisualizer : SerializedMonoBehaviour
{
    public delegate void DimensionUpdate();
    public event DimensionUpdate OnDimensionsUpdatedInInspector;
    public ClutterCube Cube = new ClutterCube();

    public void DimensionsUpdated()
    {
        OnDimensionsUpdatedInInspector.Invoke();
    }
}
