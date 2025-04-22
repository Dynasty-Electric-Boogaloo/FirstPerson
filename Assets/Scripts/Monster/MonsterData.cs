using System;
using Heatmap;
using UnityEngine;

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
    }
}