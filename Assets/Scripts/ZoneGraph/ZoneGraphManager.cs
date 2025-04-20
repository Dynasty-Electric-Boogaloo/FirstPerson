using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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

            _pathfinding = new ZonePathfinding(this);
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
        
        private void OnDrawGizmosSelected()
        {
            if (!zoneGraphData.valid)
                return;
        
            /*for (var i = 0; i < zoneGraphData.nodes.Count; i++)
            {
                if (zoneGraphData.nodes[i].room.id != 0)
                {
                    var random = new Random(zoneGraphData.nodes[i].room.id);
                    var hue = (random.Next() & 0xFF) / 255f;
                    var value = ((random.Next() & 0xFF) / 255f) * .75f + .25f;
                    Gizmos.color = Color.HSVToRGB(hue, 1, value);
                }
                else
                {
                    Gizmos.color = Color.red;
                }
            
                Gizmos.DrawSphere(zoneGraphData.nodes[i].position, .5f);
                Gizmos.color = Color.white;
                foreach (var connexion in zoneGraphData.nodes[i].connexions)
                {
                    if (connexion.id >= i)
                        Gizmos.DrawLine(zoneGraphData.nodes[i].position, zoneGraphData.nodes[connexion.id].position);
                }
            }*/
            
            /*Gizmos.color = Color.green;
            
            Rooms = new List<Room>(zoneGraphData.rooms.Count);

            foreach (var room in zoneGraphData.rooms)
            {
                Rooms.Add(room.Deserialize());
            }

            for (var i = 1; i < Rooms.Count; i++)
            {
                foreach (var nodeId in Rooms[i].Nodes)
                {
                    if (i != 0)
                    {
                        var random = new Random(i);
                        var hue = (random.Next() & 0xFF) / 255f;
                        var value = ((random.Next() & 0xFF) / 255f) * .75f + .25f;
                        Gizmos.color = Color.HSVToRGB(hue, 1, value);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
            
                    Gizmos.DrawSphere(zoneGraphData.nodes[nodeId.id].position, .5f);
                    Gizmos.color = Color.white;
                    foreach (var connexion in zoneGraphData.nodes[nodeId.id].connexions)
                    {
                        if (connexion.id >= nodeId.id)
                            Gizmos.DrawLine(zoneGraphData.nodes[nodeId.id].position, zoneGraphData.nodes[connexion.id].position);
                    }
                }
                
                foreach (var connexion in Rooms[i].EntryPoints.Keys)
                {
                    if (connexion.id >= i)
                        Gizmos.DrawLine(Rooms[i].Position, Rooms[connexion.id].Position);
                }
            }*/
        }
    }
}