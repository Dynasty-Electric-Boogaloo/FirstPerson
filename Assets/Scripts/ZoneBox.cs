using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ZoneBox : MonoBehaviour
{
    [SerializeField] private new Collider collider;

    public bool ContainsPoint(Vector3 point)
    {
        return Vector3.Distance(point, collider.ClosestPoint(point)) < 0.01f;
    }
    
#if UNITY_EDITOR
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private Vector3 _lastScale;

    private void OnEnable()
    {
        if (EditorApplication.isPlaying || ZoneGraphManager.Instance == null)
            return;
        
        ZoneGraphManager.Instance.ComputeZones();
    }

    private void OnDisable()
    {
        if (EditorApplication.isPlaying || ZoneGraphManager.Instance == null)
            return;
        
        ZoneGraphManager.Instance.ComputeZones();
    }

    private void Update()
    {
        if (EditorApplication.isPlaying || ZoneGraphManager.Instance == null)
            return;

        var size = new Vector3(
            transform.localScale.x * collider.bounds.size.x,
            transform.localScale.y * collider.bounds.size.y,
            transform.localScale.z * collider.bounds.size.z
        );
        
        if (transform.position + collider.bounds.center == _lastPosition && 
            transform.rotation == _lastRotation && 
            size == _lastScale)
            return;
        
        ZoneGraphManager.Instance.ComputeZones();
        
        _lastPosition = transform.position + collider.bounds.center;
        _lastRotation = transform.rotation;
        _lastScale = size;
    }
#endif
}