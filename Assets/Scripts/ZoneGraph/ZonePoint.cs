using System;
using UnityEditor;
using UnityEngine;

namespace ZoneGraph
{
    [ExecuteAlways, Serializable]
    public class ZonePoint : MonoBehaviour
    {
        [SerializeField] private RoomId roomId;

#if UNITY_EDITOR
        private Vector3 _lastPosition;

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

            if (transform.position == _lastPosition)
                return;
        
            ZoneGraphComputer.Instance.ComputeZones(true);
            _lastPosition = transform.position;
        }
#endif

        public void SetRoom(RoomId id)
        {
            roomId = id;
        }
    }
}