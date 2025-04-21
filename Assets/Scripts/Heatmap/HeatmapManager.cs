using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using ZoneGraph;

namespace Heatmap
{
    public class HeatmapManager : MonoBehaviour
    {
        private static HeatmapManager _instance;
        [SerializeField] private float playerHeat;
        
        private Dictionary<RoomId, HeatmapData> _baseRoomMaps;
        private Dictionary<RoomId, HeatmapData> _roomMaps;
        private RoomId _currentRecording;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;

            _baseRoomMaps = new Dictionary<RoomId, HeatmapData>();
            _roomMaps = new Dictionary<RoomId, HeatmapData>();

            _currentRecording.id = -1;
        }

        private void Start()
        {
            var nodes = ZoneGraphManager.Instance.Nodes;
            var rooms = ZoneGraphManager.Instance.Rooms;

            for (var i = 0; i < ZoneGraphManager.Instance.Rooms.Count; i++)
            {
                var heatmap = new HeatmapData($"Base map {i}");

                foreach (var nodeId in rooms[i].Nodes)
                {
                    if (nodes[nodeId.id].Heat <= 0)
                        continue;
                    
                    heatmap.Data[nodeId] = nodes[nodeId.id].Heat;
                }
                
                _baseRoomMaps.Add(new RoomId(i), heatmap);
            }
        }

        private void Update()
        {
            if (_currentRecording.id < 0)
                return;

            var playerRoom = ZoneGraphManager.Pathfinding.GetPointRoom(PlayerRoot.Position);
            var playerNode = ZoneGraphManager.Pathfinding.GetPointClosestNode(PlayerRoot.Position, playerRoom);

            var heatData = _roomMaps[_currentRecording].Data;

            heatData.TryAdd(playerNode, 0);
            heatData[playerNode] = Mathf.Clamp01(heatData[playerNode] + playerHeat * Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public static void GetRoomHeatmap(RoomId roomId, ref HeatmapData outputHeatmap)
        {
            if (_instance == null)
                return;
            
            if (!_instance._roomMaps.ContainsKey(roomId))
            {
                var heatmap = new HeatmapData($"Map {roomId.id}");
                
                _instance._baseRoomMaps[roomId].CopyTo(heatmap);
                _instance._roomMaps[roomId] = heatmap;
            }
            
            _instance._roomMaps[roomId].CopyTo(outputHeatmap);
        }

        public static void StartRecording(RoomId roomId)
        {
            if (_instance == null)
                return;

            if (_instance._currentRecording == roomId)
                return;

            if (!_instance._roomMaps.ContainsKey(roomId))
                _instance._roomMaps[roomId] = new HeatmapData($"Map {roomId.id}");

            _instance._currentRecording = roomId;
            _instance._baseRoomMaps[roomId].CopyTo(_instance._roomMaps[roomId]);
        }

        public static void StopRecording()
        {
            if (_instance == null)
                return;

            if (_instance._currentRecording.id < 0)
                return;

            _instance._currentRecording.id = -1;
        }
    }
}