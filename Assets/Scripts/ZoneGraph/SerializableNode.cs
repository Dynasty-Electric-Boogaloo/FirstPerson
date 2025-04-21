using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    [Serializable]
    public struct SerializableNode
    {
        public Vector3 position;
        public float heat;
        public RoomId room;
        public List<NodeId> connexions;

        public Node Deserialize()
        {
            return new Node
            {
                Position = position,
                Heat = heat,
                Room = room,
                Connexions = new HashSet<NodeId>(connexions)
            };
        }
    }
}