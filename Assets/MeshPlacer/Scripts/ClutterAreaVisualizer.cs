using UnityEngine;
[RequireComponent(typeof(ClutterAreaComponent))]
public class ClutterAreaVisualizer : MonoBehaviour
{
    public delegate void DimensionUpdate();
    public event DimensionUpdate OnDimensionsUpdatedInInspector;

    public void DimensionsUpdated()
    {
        OnDimensionsUpdatedInInspector.Invoke();
    }
}
