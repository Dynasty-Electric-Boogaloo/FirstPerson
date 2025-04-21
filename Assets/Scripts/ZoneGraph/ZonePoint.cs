using System;
using UnityEditor;
using UnityEngine;

namespace ZoneGraph
{
    [ExecuteAlways, Serializable]
    public class ZonePoint : MonoBehaviour
    {
        [SerializeField] private RoomId roomId;
        [SerializeField] private float heat;

        public float Heat => heat;

        public void SetRoom(RoomId id)
        {
            roomId = id;
        }
    }
}