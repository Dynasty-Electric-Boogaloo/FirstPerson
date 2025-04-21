using UnityEngine;

namespace Monster
{
    public class MonsterData
    {
        public Rigidbody Rigidbody;
        public Vector3 TargetPoint;
        public bool Grounded;
        public bool Chasing;
        public bool Searching;
        public float StateTime;
    }
}