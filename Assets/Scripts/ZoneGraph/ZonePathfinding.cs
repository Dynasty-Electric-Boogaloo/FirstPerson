using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    public class ZonePathfinding
    {
        private ZoneGraphManager _graph;
        private HashSet<int> _openBuffer;
        private Dictionary<int, int> _cameFromBuffer;
        private Dictionary<int, float> _costBuffer;
        private Dictionary<int, float> _guessCostBuffer;
        private ZoneBox[] _zones;
        private bool _debugDrawLines;
        
        public ZonePathfinding(ZoneGraphManager graphManager, bool debug = false)
        {
            _graph = graphManager;
            _openBuffer = new HashSet<int>();
            _cameFromBuffer = new Dictionary<int, int>();
            _costBuffer = new Dictionary<int, float>();
            _guessCostBuffer = new Dictionary<int, float>();
            _debugDrawLines = debug;
            _zones = Object.FindObjectsByType<ZoneBox>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        }

        public NodeId PathfindToPoint(Vector3 currentPoint, Vector3 targetPoint)
        {
            var currentRoom = GetPointRoom(currentPoint);
            var targetRoom = GetPointRoom(targetPoint);

            var currentNode = GetPointClosestNode(currentPoint, currentRoom);
            
            if (currentNode.id < 0)
                return currentNode;

            if (currentRoom != targetRoom)
            {
                var travelRoom = PathfindToRoom(currentRoom, targetRoom);

                if (travelRoom.id < 0)
                    return new NodeId(-1);
                
                var targetNode = GetClosestConnexionToRoom(currentNode, currentRoom, travelRoom);
                
                if (targetNode.id < 0)
                    return targetNode;

                if (currentNode == targetNode)
                    targetNode = GetPointClosestNode(currentPoint, travelRoom);

                if (targetNode.id < 0)
                    return targetNode;
                
                return PathfindToPointInRoom(currentNode, targetNode);
            }
            else
            {
                var targetNode = GetPointClosestNode(targetPoint, targetRoom);
                
                if (targetNode.id < 0)
                    return targetNode;
                
                return PathfindToPointInRoom(currentNode, targetNode);
            }
        }
        
        public RoomId GetPointRoom(Vector3 position)
        {
            var room = 0;
            var priority = int.MinValue;
                
            for (var i = 0; i < _zones.Length; i++)
            {
                if (_zones[i].Priority <= priority)
                    continue;
                    
                if (!_zones[i].ContainsPoint(position))
                    continue;

                room = _zones[i].zoneId;
                priority = _zones[i].Priority;
            }

            return new RoomId(room);
        }

        public NodeId GetPointClosestNode(Vector3 position, RoomId room)
        {
            var closestNodeId = new NodeId(-1);
            var closestDistance = float.PositiveInfinity;
            
            foreach (var nodeId in _graph.Rooms[room.id].Nodes)
            {
                var distance = SqrDistance(_graph.Nodes[nodeId.id].Position, position);

                if (!(distance < closestDistance)) 
                    continue;
                
                closestDistance = distance;
                closestNodeId = nodeId;
            }

            return closestNodeId;
        }

        public NodeId PathfindToPointInRoom(NodeId startNode, NodeId endNode)
        {
            var nodes = _graph.Nodes;
            var endPosition = nodes[endNode.id].Position;
            
            _openBuffer.Clear();
            _costBuffer.Clear();
            _guessCostBuffer.Clear();
            _cameFromBuffer.Clear();
            
            _openBuffer.Add(startNode.id);
            _costBuffer[startNode.id] = 0;
            _guessCostBuffer[startNode.id] = SqrDistance(nodes[startNode.id].Position, endPosition);

            while (_openBuffer.Count > 0)
            {
                var current = GetBestOpenNode();

                if (current == endNode.id)
                {
                    if (_debugDrawLines)
                        DrawNodePath(startNode.id, endNode.id);
                    return new NodeId(ReconstructPath(startNode.id, endNode.id));
                }

                _openBuffer.Remove(current);
                foreach (var neighbor in nodes[current].Connexions)
                {
                    _costBuffer.TryAdd(neighbor.id, float.PositiveInfinity);
                    var cost = _costBuffer[current] + SqrDistance(nodes[current].Position, nodes[neighbor.id].Position);

                    if (cost > _costBuffer[neighbor.id])
                        continue;

                    UpdateNeighborNode(current, neighbor.id, cost, SqrDistance(nodes[neighbor.id].Position, endPosition));
                }
            }

            return new NodeId(-1);
        }

        public RoomId PathfindToRoom(RoomId startRoom, RoomId endRoom)
        {
            var rooms = _graph.Rooms;
            var endPosition = rooms[endRoom.id].Position;
            
            _openBuffer.Clear();
            _costBuffer.Clear();
            _guessCostBuffer.Clear();
            _cameFromBuffer.Clear();
            
            _openBuffer.Add(startRoom.id);
            _costBuffer[startRoom.id] = 0;
            _guessCostBuffer[startRoom.id] = SqrDistance(rooms[startRoom.id].Position, endPosition);

            while (_openBuffer.Count > 0)
            {
                var current = GetBestOpenNode();

                if (current == endRoom.id)
                {
                    if (_debugDrawLines)
                        DrawRoomPath(startRoom.id, endRoom.id);
                    return new RoomId(ReconstructPath(startRoom.id, endRoom.id));
                }

                _openBuffer.Remove(current);
                foreach (var neighbor in rooms[current].EntryPoints.Keys)
                {
                    _costBuffer.TryAdd(neighbor.id, float.PositiveInfinity);
                    var cost = _costBuffer[current] + SqrDistance(rooms[current].Position, rooms[neighbor.id].Position);

                    if (cost > _costBuffer[neighbor.id])
                        continue;

                    UpdateNeighborNode(current, neighbor.id, cost, SqrDistance(rooms[neighbor.id].Position, endPosition));
                }
            }

            return new RoomId(-1);
        }

        public NodeId GetClosestConnexionToRoom(NodeId startNode, RoomId currentRoom, RoomId targetRoom)
        {
            var closestNode = new NodeId(-1);
            var closestDistance = float.PositiveInfinity;
            
            var nodes = _graph.Nodes;
            var rooms = _graph.Rooms;

            foreach (var nodeId in rooms[currentRoom.id].EntryPoints[targetRoom])
            {
                var distance = SqrDistance(nodes[startNode.id].Position, nodes[nodeId.id].Position);
                
                if (distance > closestDistance)
                    continue;

                closestNode = nodeId;
                closestDistance = distance;
            }

            return closestNode;
        }
        
        private int GetBestOpenNode()
        {
            var bestNode = -1;
            var bestGuessCost = float.PositiveInfinity;
            
            foreach (var node in _openBuffer)
            {
                if (_guessCostBuffer[node] > bestGuessCost)
                    continue;

                bestNode = node;
                bestGuessCost = _guessCostBuffer[node];
            }

            return bestNode;
        }

        private void DrawNodePath(int startId, int endId)
        {
            var last = endId;
            var current = endId;

            while (current != startId)
            {
                last = current;
                current = _cameFromBuffer[current];

                Debug.DrawLine(_graph.Nodes[current].Position + Vector3.up, _graph.Nodes[last].Position + Vector3.up, Color.white, .2f);
            }
        }
        
        private void DrawRoomPath(int startId, int endId)
        {
            var last = endId;
            var current = endId;

            while (current != startId)
            {
                last = current;
                current = _cameFromBuffer[current];

                Debug.DrawLine(_graph.Rooms[current].Position, _graph.Rooms[last].Position, Color.green, .2f);
            }
        }

        private int ReconstructPath(int startId, int endId)
        {
            var last = endId;
            var current = endId;

            while (current != startId)
            {
                last = current;
                current = _cameFromBuffer[current];
            }

            return last;
        }

        private void UpdateNeighborNode(int current, int neighbor, float cost, float guessCost)
        {
            _costBuffer[neighbor] = cost;
            _guessCostBuffer[neighbor] = cost + guessCost;
            _cameFromBuffer[neighbor] = current;
            _openBuffer.Add(neighbor);
        }

        private static float SqrDistance(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }
    }
}