using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace ZoneGraph
{
    [ExecuteInEditMode]
    public class ZoneGraphComputer : MonoBehaviour
    {
        #if UNITY_EDITOR
        private static ZoneGraphComputer _instance;
        [SerializeField] private bool enableAutoZoneCompute;
        [SerializeField] private float connexionDistance;
        [SerializeField] private float collisionHeight;
        [SerializeField] private float collisionOffset;
        [SerializeField] private float collisionRadius;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private ZoneGraphData outputGraphData;
        private List<SerializableNode> _nodes = new ();
        private List<SerializableRoom> _rooms = new ();

        public static ZoneGraphComputer Instance => _instance;
        
        private ZonePathfinding _pathfinding;
        private RaycastHit[] _collisionBuffer;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            _collisionBuffer = new RaycastHit[1];
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void OnEnable()
        {
            if (_instance == null)
                _instance = this;
        }

        private void OnDisable()
        {
            if (_instance == this)
                _instance = null;
        }

        public void ComputeZones(bool auto)
        {
            if (!enableAutoZoneCompute && auto)
                return;

            outputGraphData.valid = false;
        
            _nodes.Clear();
            _rooms.Clear();

            var zones = FindObjectsByType<ZoneBox>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
            var zonePoints = FindObjectsByType<ZonePoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            for (var i = 0; i < zones.Length; i++)
            {
                zones[i].zoneId = i + 1;
            }

            var sqrDistance = connexionDistance * connexionDistance;
            
            _rooms.Add(new SerializableRoom
            {
                position = Vector3.zero,
                nodes = new List<NodeId>(),
                entryPointKeys = new List<RoomId>(),
                entryPointValues = new List<SerializableRoom.EntryPoints>()
            });
            
            for (var i = 0; i < zones.Length; i++)
            {
                _rooms.Add(new SerializableRoom()
                {
                    position = zones[i].transform.position,
                    nodes = new List<NodeId>(),
                    entryPointKeys = new List<RoomId>(),
                    entryPointValues = new List<SerializableRoom.EntryPoints>()
                });
            }

            foreach (var zonePoint in zonePoints)
            {
                var connexions = new HashSet<NodeId>();
            
                var bottomPoint = zonePoint.transform.position + Vector3.up * collisionOffset;
                var topPoint = zonePoint.transform.position + Vector3.up * (collisionOffset + collisionHeight);
            
                for (var i = 0; i < zonePoints.Length; i++)
                {
                    if (zonePoint == zonePoints[i])
                        continue;

                    var diff = zonePoints[i].transform.position - zonePoint.transform.position;
                    var distance = diff.sqrMagnitude;
                    if (distance > sqrDistance)
                        continue;

                    //var count = Physics.CapsuleCastNonAlloc(bottomPoint, topPoint, collisionRadius, diff.normalized, _collisionBuffer, distance, collisionMask);

                    //if (count <= 0)
                        connexions.Add(new NodeId(i));
                }

                var room = GetPointRoom(zonePoint.transform.position, zones);

                if (room.id == 0)
                    Debug.LogWarning($"Node {zonePoint.name} is not in any room! Please adjust _rooms or the node's position!");
                
                zonePoint.SetRoom(room);
                _rooms[room.id].nodes.Add(new NodeId(_nodes.Count));

                _nodes.Add(
                    new SerializableNode()
                    {
                        position = zonePoint.transform.position,
                        room = room,
                        connexions = new List<NodeId>(connexions)
                    });
            }

            for (var i = 0; i < _nodes.Count; i++)
            {
                foreach (var connexion in _nodes[i].connexions)
                {
                    var nodeRoom = _nodes[i].room;
                    var otherRoom = _nodes[connexion.id].room;
                    
                    if (nodeRoom == otherRoom)
                        continue;

                    var index = _rooms[nodeRoom.id].entryPointKeys.IndexOf(otherRoom);

                    if (index >= 0)
                    {
                        _rooms[nodeRoom.id].entryPointValues[index].nodes.Add(new NodeId(i));
                    }
                    else
                    {
                        _rooms[nodeRoom.id].entryPointKeys.Add(otherRoom);
                        _rooms[nodeRoom.id].entryPointValues.Add(
                            new SerializableRoom.EntryPoints
                            {
                                nodes = new List<NodeId>(1)
                            });
                        
                        _rooms[nodeRoom.id].entryPointValues[_rooms[nodeRoom.id].entryPointKeys.Count - 1].nodes.Add(new NodeId(i));
                    }
                }
            }

            outputGraphData.nodes = _nodes;
            outputGraphData.rooms = _rooms;

            outputGraphData.valid = true;
        }

        public RoomId GetPointRoom(Vector3 position, ZoneBox[] zones)
        {
            var room = 0;
            var priority = int.MinValue;
                
            for (var i = 0; i < zones.Length; i++)
            {
                if (zones[i].Priority <= priority)
                    continue;
                    
                if (!zones[i].ContainsPoint(position))
                    continue;

                room = zones[i].zoneId;
                priority = zones[i].Priority;
            }

            return new RoomId(room);
        }

        private void OnDrawGizmosSelected()
        {
            if (!outputGraphData.valid)
                return;
        
            for (var i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].room.id != 0)
                {
                    var random = new Random(_nodes[i].room.id);
                    var hue = (random.Next() & 0xFF) / 255f;
                    var value = ((random.Next() & 0xFF) / 255f) * .75f + .25f;
                    Gizmos.color = Color.HSVToRGB(hue, 1, value);
                }
                else
                {
                    Gizmos.color = Color.red;
                }
            
                Gizmos.DrawSphere(_nodes[i].position, .5f);
                Gizmos.color = Color.white;
                foreach (var connexion in _nodes[i].connexions)
                {
                    if (connexion.id >= i)
                        Gizmos.DrawLine(_nodes[i].position, _nodes[connexion.id].position);
                }
            }

            Gizmos.color = Color.green;

            for (var i = 1; i < _rooms.Count; i++)
            {
                foreach (var connexion in _rooms[i].entryPointKeys)
                {
                    if (connexion.id >= i)
                        Gizmos.DrawLine(_rooms[i].position, _rooms[connexion.id].position);
                }
            }
        }
        #endif
    }
}