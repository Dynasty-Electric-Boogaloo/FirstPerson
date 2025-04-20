using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    [Serializable]
    public struct SerializableRoom
    {
        [Serializable]
        public struct EntryPoints
        {
            public List<NodeId> nodes;
        }
        
        public Vector3 position;
        public List<NodeId> nodes;
        public List<RoomId> entryPointKeys;
        public List<EntryPoints> entryPointValues;

        public Room Deserialize()
        {
            var entryPointsDictionnary = new Dictionary<RoomId, List<NodeId>>(entryPointKeys.Count);

            for (var i = 0; i < entryPointKeys.Count; i++)
            {
                entryPointsDictionnary[entryPointKeys[i]] = new List<NodeId>(entryPointValues[i].nodes);
            }

            return new Room
            {
                Position = position,
                Nodes = new List<NodeId>(nodes),
                EntryPoints = entryPointsDictionnary
            };
        }
    }
}