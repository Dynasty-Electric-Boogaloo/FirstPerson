using UnityEngine;

namespace ZoneGraph
{
    [ExecuteAlways]
    public class ZoneBox : MonoBehaviour
    {
        [SerializeField] private int priority;
        public int zoneId;
        private Collider[] _colliders;

        public int Priority => priority;

        private void Awake()
        {
            _colliders = GetComponents<Collider>();
        }

        private void OnEnable()
        {
            _colliders ??= GetComponents<Collider>();
        }

        public bool ContainsPoint(Vector3 point)
        {
            var closestDistance = float.PositiveInfinity;

            foreach (var col in _colliders)
            {
                var distance = Vector3.Distance(point, col.ClosestPoint(point));
                
                if (distance < closestDistance)
                    closestDistance = distance;
            }
            
            return closestDistance < 0.01f;
        }

        public void RefreshColliderList()
        {
            _colliders = GetComponents<Collider>();
        }
    }
}