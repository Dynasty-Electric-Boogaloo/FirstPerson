using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZoneGraph
{
    public struct Node
    {
        public Vector3 Position;
        public float Heat;
        public RoomId Room;
        public HashSet<NodeId> Connexions;

        public SerializableNode Serialize()
        {
            return new SerializableNode
            {
                position = Position,
                heat = Heat,
                room = Room,
                connexions = new List<NodeId>(Connexions)
            };
        }
    }
}