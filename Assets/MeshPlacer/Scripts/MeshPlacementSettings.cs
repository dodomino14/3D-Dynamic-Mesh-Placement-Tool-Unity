using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
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
    public GUIStyle TextStyle {get => _textStyle;}
    public bool HideGizmosOnEnable {get => _hideGizmosOnEnable;}
    public KeyControl XKey {get => Keyboard.current[_xKey];}
    public KeyControl YKey {get => Keyboard.current[_yKey];}
    public KeyControl ZKey {get => Keyboard.current[_zKey];}
    [SerializeField] private Key _yKey, _xKey, _zKey;
    [SerializeField] private float _snapIncrement;
    [SerializeField] private float _handleSize;
    [SerializeField] private Color _defaultColor, _gridColor, _errorColor, _centerColor, _textColor;
    [SerializeField] private GUIStyle _textStyle;
    [SerializeField] private bool _hideGizmosOnEnable;
}
