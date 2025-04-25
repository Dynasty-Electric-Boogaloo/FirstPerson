using System;

namespace ZoneGraph
{
    [Serializable]
    public struct NodeId
    {
        public int id;

        public NodeId(int index)
        {
            id = index;
        }
        
        public static bool operator ==(NodeId a, NodeId b)
        {
            return a.id == b.id;
        }

        public static bool operator !=(NodeId a, NodeId b)
        {
            return a.id != b.id;
        }
        
        public bool Equals(NodeId other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is NodeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}