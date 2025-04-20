using System;

namespace ZoneGraph
{
    [Serializable]
    public struct RoomId
    {
        public int id;
        
        public RoomId(int index)
        {
            id = index;
        }

        public static bool operator ==(RoomId a, RoomId b)
        {
            return a.id == b.id;
        }

        public static bool operator !=(RoomId a, RoomId b)
        {
            return a.id != b.id;
        }
        
        public bool Equals(RoomId other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is RoomId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}