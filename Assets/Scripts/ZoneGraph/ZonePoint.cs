using System;
using UnityEditor;
using UnityEngine;

namespace ZoneGraph
{
    [ExecuteAlways, Serializable]
    public class ZonePoint : MonoBehaviour
    {
        [SerializeField] private float heat;

        public float Heat => heat;
    }
}