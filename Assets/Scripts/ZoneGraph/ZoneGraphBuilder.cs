using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace ZoneGraph
{
    [ExecuteInEditMode]
    public class ZoneGraphBuilder : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField] private float connexionDistance;
        [SerializeField] private float collisionHeight;
        [SerializeField] private float collisionOffset;
        [SerializeField] private float collisionRadius;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private ZoneGraphData outputGraphData;

        public void ComputeZones()
        {
            var collisionBuffer = new RaycastHit[1];
            outputGraphData.valid = false;

            var zones = FindObjectsByType<ZoneBox>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
            var zonePoints = new List<ZonePoint>(FindObjectsByType<ZonePoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
            
            var nodes = new List<SerializableNode>(zonePoints.Count);
            var rooms = new List<SerializableRoom>(zones.Length);
            
            for (var i = 0; i < zones.Length; i++)
            {
                zones[i].zoneId = i + 1;
            }

            var sqrDistance = connexionDistance * connexionDistance;
            
            rooms.Add(new SerializableRoom
            {
                position = Vector3.zero,
                nodes = new List<NodeId>(),
                entryPointKeys = new List<RoomId>(),
                entryPointValues = new List<SerializableRoom.EntryPoints>()
            });
            
            for (var i = 0; i < zones.Length; i++)
            {
                rooms.Add(new SerializableRoom()
                {
                    position = zones[i].transform.position,
                    nodes = new List<NodeId>(),
                    entryPointKeys = new List<RoomId>(),
                    entryPointValues = new List<SerializableRoom.EntryPoints>()
                });
            }

            for (var j = 0; j < zonePoints.Count; j++)
            {
                var zonePoint = zonePoints[j];
                var connexions = new HashSet<NodeId>();
            
                var bottomPoint = zonePoint.transform.position + Vector3.up * collisionOffset;
                var topPoint = zonePoint.transform.position + Vector3.up * (collisionOffset + collisionHeight);
            
                for (var i = 0; i < zonePoints.Count; i++)
                {
                    if (zonePoint == zonePoints[i])
                        continue;

                    var diff = zonePoints[i].transform.position - zonePoint.transform.position;
                    var distance = diff.sqrMagnitude;
                    if (distance > sqrDistance)
                        continue;

                    var count = Physics.CapsuleCastNonAlloc(bottomPoint, topPoint, collisionRadius, diff.normalized, collisionBuffer, Mathf.Sqrt(distance), collisionMask);
                    
                    if (count <= 0)
                        connexions.Add(new NodeId(i));
                }

                var room = GetPointRoom(zonePoint.transform.position, zones);

                if (room.id == 0)
                    Debug.LogWarning($"Node {zonePoint.name} is not in any room! Please adjust rooms or the node's position!");

                if (connexions.Count <= 0)
                {
                    foreach (var node in nodes)
                    {
                        for (var i = 0; i < node.connexions.Count; i++)
                        {
                            if (node.connexions[i].id < j)
                                continue;
                            
                            var nodeId = node.connexions[i];
                            nodeId.id--;
                            node.connexions[i] = nodeId;
                        }
                    }
                    
                    zonePoints.RemoveAt(j--);
                    continue;
                }

                rooms[room.id].nodes.Add(new NodeId(nodes.Count));
                
                nodes.Add(
                    new SerializableNode()
                    {
                        position = zonePoint.transform.position,
                        heat = Mathf.Clamp01(zonePoint.Heat),
                        room = room,
                        connexions = new List<NodeId>(connexions)
                    });
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                foreach (var connexion in nodes[i].connexions)
                {
                    var nodeRoom = nodes[i].room;
                    
                    var otherRoom = nodes[connexion.id].room;
                    
                    if (nodeRoom == otherRoom)
                        continue;

                    var index = rooms[nodeRoom.id].entryPointKeys.IndexOf(otherRoom);

                    if (index >= 0)
                    {
                        rooms[nodeRoom.id].entryPointValues[index].nodes.Add(new NodeId(i));
                    }
                    else
                    {
                        rooms[nodeRoom.id].entryPointKeys.Add(otherRoom);
                        rooms[nodeRoom.id].entryPointValues.Add(
                            new SerializableRoom.EntryPoints
                            {
                                nodes = new List<NodeId>(1)
                            });
                        
                        rooms[nodeRoom.id].entryPointValues[rooms[nodeRoom.id].entryPointKeys.Count - 1].nodes.Add(new NodeId(i));
                    }
                }
            }

            outputGraphData.nodes = nodes;
            outputGraphData.rooms = rooms;
            outputGraphData.valid = true;
            EditorUtility.SetDirty(outputGraphData);
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
        
            for (var i = 0; i < outputGraphData.nodes.Count; i++)
            {
                if (outputGraphData.nodes[i].room.id != 0)
                {
                    var random = new Random(outputGraphData.nodes[i].room.id);
                    var hue = (random.Next() & 0xFF) / 255f;
                    var value = ((random.Next() & 0xFF) / 255f) * .75f + .25f;
                    Gizmos.color = Color.HSVToRGB(hue, 1, value);
                }
                else
                {
                    Gizmos.color = Color.red;
                }
            
                Gizmos.DrawSphere(outputGraphData.nodes[i].position, .25f + outputGraphData.nodes[i].heat * 2.5f);
                Gizmos.color = Color.white;
                foreach (var connexion in outputGraphData.nodes[i].connexions)
                {
                    if (connexion.id >= i)
                        Gizmos.DrawLine(outputGraphData.nodes[i].position, outputGraphData.nodes[connexion.id].position);
                }
            }

            Gizmos.color = Color.green;

            for (var i = 1; i < outputGraphData.rooms.Count; i++)
            {
                foreach (var connexion in outputGraphData.rooms[i].entryPointKeys)
                {
                    if (connexion.id >= i)
                        Gizmos.DrawLine(outputGraphData.rooms[i].position, outputGraphData.rooms[connexion.id].position);
                }
            }
        }
        #endif
    }
}