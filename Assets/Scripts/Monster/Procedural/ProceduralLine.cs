using System;
using UnityEngine;

namespace Monster.Procedural
{
    public class ProceduralLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;
        [SerializeField] private Transform[] points;

        private void Start()
        {
            line.positionCount = points.Length;
        }

        private void Update()
        {
            for (var i = 0; i < points.Length; i++)
            {
                line.SetPosition(i, points[i].position);
            }
        }
    }
}