using System;
using Heatmap;
using UnityEngine;
using UnityEngine.Serialization;
using ZoneGraph;

namespace Monster
{
    [Serializable]
    public class MonsterData
    {
        public Rigidbody rigidbody;
        public Vector3 targetPoint;
        public bool grounded;
        public bool chasing;
        public bool searching;
        public float stateTime;
        public HeatmapData Heatmap;
        public float hitStunTimer;
        public float watchTimer;
        public float chaseTimer;
        public NodeId targetNode;
    }
}