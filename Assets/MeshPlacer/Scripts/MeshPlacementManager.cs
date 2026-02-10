using UnityEngine;
using UnityEditor;
[ExecuteAlways]
public class MeshPlacementManager : MonoBehaviour
{
    public static MeshPlacementSettings Settings {get => GetSettings();}
    [SerializeField] private MeshPlacementSettings _settings;
    private static string _settingsPath;
    public static MeshPlacementManager Instance;

    void OnEnable()
    {
        if(Instance) Debug.LogWarning("Duplicate Mesh Placement Manager discovered in " + gameObject.name);
        else Instance = this;
    }
    void OnValidate()
    {
        if(_settings != null) _settingsPath = AssetDatabase.GetAssetPath(_settings);
    }
    void OnDisable()
    {
        Instance = null;
    }

    private static MeshPlacementSettings GetSettings()
    {
        if(Instance != null) return Instance._settings;
        return AssetDatabase.LoadAssetAtPath<MeshPlacementSettings>(_settingsPath);
    }
}
