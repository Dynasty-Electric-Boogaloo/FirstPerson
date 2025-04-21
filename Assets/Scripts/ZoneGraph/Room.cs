using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    public struct Room
    {
        public Vector3 Position;
        public List<NodeId> Nodes;
        public Dictionary<RoomId, List<NodeId>> EntryPoints;

        public SerializableRoom Serialize()
        {
            var entryPointValues = new List<SerializableRoom.EntryPoints>(EntryPoints.Count);

            foreach (var entryPoint in EntryPoints.Values)
            {
                entryPointValues.Add(new SerializableRoom.EntryPoints
                {
                    nodes = new List<NodeId>(entryPoint)
                });
            }

            return new SerializableRoom
            {
                position = Position,
                nodes = new List<NodeId>(Nodes),
                entryPointKeys = new List<RoomId>(EntryPoints.Keys),
                entryPointValues = entryPointValues
            };
        }
    }
}