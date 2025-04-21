using UnityEditor;
using UnityEngine;

namespace ZoneGraph
{
    [ExecuteAlways]
    public class ZoneBox : MonoBehaviour
    {
        [SerializeField] private new Collider collider;
        [SerializeField] private int priority;
        public int zoneId;

        public int Priority => priority;

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
            if (EditorApplication.isPlaying || ZoneGraphComputer.Instance == null)
                return;
        
            ZoneGraphComputer.Instance.ComputeZones(true);
        }

        private void OnDisable()
        {
            if (EditorApplication.isPlaying || ZoneGraphComputer.Instance == null)
                return;
        
            ZoneGraphComputer.Instance.ComputeZones(true);
        }

        private void Update()
        {
            if (EditorApplication.isPlaying || ZoneGraphComputer.Instance == null)
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
        
            ZoneGraphComputer.Instance.ComputeZones(true);
        
            _lastPosition = transform.position + collider.bounds.center;
            _lastRotation = transform.rotation;
            _lastScale = size;
        }
#endif
    }
}