using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    public class ZoneGraphManager : MonoBehaviour
    {
        private static ZoneGraphManager _instance;
        [SerializeField] private ZoneGraphData zoneGraphData;
        [HideInInspector] public List<Node> Nodes = new ();
        [HideInInspector] public List<Room> Rooms = new ();
        private ZonePathfinding _pathfinding;
        
        public static ZoneGraphManager Instance => _instance;
        public static ZonePathfinding Pathfinding => _instance ? _instance._pathfinding : null;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            Nodes = new List<Node>(zoneGraphData.nodes.Count);

            foreach (var node in zoneGraphData.nodes)
            {
                Nodes.Add(node.Deserialize());
            }
            
            Rooms = new List<Room>(zoneGraphData.rooms.Count);

            foreach (var room in zoneGraphData.rooms)
            {
                Rooms.Add(room.Deserialize());
            }

            _pathfinding = new ZonePathfinding(this, true);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
        
        public Vector3 GetNodePosition(NodeId nodeId)
        {
            return nodeId.id < 0 ? Vector3.zero : Nodes[nodeId.id].Position;
        }
    }
}