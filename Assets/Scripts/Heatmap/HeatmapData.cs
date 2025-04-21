using System.Collections.Generic;
using ZoneGraph;

namespace Heatmap
{
    public class HeatmapData
    {
        public string Name;
        public Dictionary<NodeId, float> Data;

        public HeatmapData(string name)
        {
            Name = name;
            Data = new Dictionary<NodeId, float>();
        }

        public void CopyTo(HeatmapData other)
        {
            other.Data ??= new Dictionary<NodeId, float>();
            other.Data.Clear();

            foreach (var pair in Data)
            {
                other.Data[pair.Key] = pair.Value;
            }
        }
    }
}