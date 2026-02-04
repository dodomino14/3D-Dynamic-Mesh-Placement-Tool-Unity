using UnityEngine;
[ExecuteAlways]
public class MeshPlacementManager : MonoBehaviour
{
    public static MeshPlacementSettings Settings {get => Instance._settings;}
    [SerializeField] private MeshPlacementSettings _settings;

    public static MeshPlacementManager Instance;

    void OnEnable()
    {
        if(Instance) Debug.LogWarning("Duplicate Mesh Placement Manager discovered in " + gameObject.name);
        else Instance = this;
    }
    void OnDisable()
    {
        Instance = null;
    }
}
