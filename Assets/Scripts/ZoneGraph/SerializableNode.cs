using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    [Serializable]
    public struct SerializableNode
    {
        public Vector3 position;
        public RoomId room;
        public List<NodeId> connexions;

        public Node Deserialize()
        {
            return new Node
            {
                Position = position,
                Room = room,
                Connexions = new HashSet<NodeId>(connexions)
            };
        }
    }
}