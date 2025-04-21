using System.Collections.Generic;
using UnityEngine;

namespace ZoneGraph
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Zone Graph Data", fileName = "Zone Graph Data")]
    public class ZoneGraphData : ScriptableObject
    {
        public bool valid;
        public List<SerializableNode> nodes = new ();
        public List<SerializableRoom> rooms = new ();
    }
}