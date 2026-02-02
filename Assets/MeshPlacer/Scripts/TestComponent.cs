using UnityEngine;
using Sirenix.OdinInspector;
public class TestComponent : MonoBehaviour
{
    [OnValueChanged("Testing")]
    public Vector3 Test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Testing()
    {
        Debug.Log("Uh oh");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
