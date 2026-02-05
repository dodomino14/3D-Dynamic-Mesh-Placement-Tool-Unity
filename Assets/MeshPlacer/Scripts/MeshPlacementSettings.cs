using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Clutter/Settings", fileName = "New Mesh Placement Settings")]
public class MeshPlacementSettings : ScriptableObject
{
    public float SnapIncrement {get => _snapIncrement;}
    public float HandleSize {get => _handleSize;}
    public Color DefaultColor {get => _defaultColor;}
    public Color ErrorColor {get => _errorColor;}
    public Color GridColor {get => _gridColor;}
    public Color CenterColor {get => _gridColor;}
    public Color TextColor {get => _textColor;}

    [SerializeField] private float _snapIncrement;
    [SerializeField] private float _handleSize;
    [SerializeField] private Color _defaultColor, _gridColor, _errorColor, _centerColor, _textColor;
}
