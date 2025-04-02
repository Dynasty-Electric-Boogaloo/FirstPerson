using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[ExecuteAlways]
public class ZoneGraphManager : MonoBehaviour
{
    [Serializable]
    private struct Node
    {
        public ZonePoint point;
        public int[] connexions;
    }
    
    private static ZoneGraphManager _instance;
    [SerializeField] private float connexionDistance;
    [SerializeField] private float collisionHeight;
    [SerializeField] private float collisionOffset;
    [SerializeField] private float collisionRadius;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField, HideInInspector] private List<Node> nodes;
    [SerializeField, HideInInspector] private ZoneBox[] zones;
    
    public static ZoneGraphManager Instance => _instance;
    private RaycastHit[] _collisionBuffer;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
        _collisionBuffer = new RaycastHit[1];
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
    
    #if UNITY_EDITOR
    private void OnEnable()
    {
        if (_instance == null)
            _instance = this;
    }

    private void OnDisable()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    
    #endif

    public void ComputeZones()
    {
        nodes.Clear();

        zones = FindObjectsByType<ZoneBox>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var zonePoints = FindObjectsByType<ZonePoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var zonePoint in zonePoints)
        {
            var connexions = new List<int>();
            
            var bottomPoint = zonePoint.transform.position + Vector3.up * collisionOffset;
            var topPoint = zonePoint.transform.position + Vector3.up * (collisionOffset + collisionHeight);
            
            for (var i = 0; i < zonePoints.Length; i++)
            {
                if (zonePoint == zonePoints[i])
                    continue;

                var diff = zonePoints[i].transform.position - zonePoint.transform.position;
                var distance = diff.magnitude;
                if (distance > connexionDistance)
                    continue;

                var count = Physics.CapsuleCastNonAlloc(bottomPoint, topPoint, collisionRadius, diff.normalized, _collisionBuffer, distance, collisionMask);

                if (count <= 0)
                    connexions.Add(i);
            }

            zonePoint.SetRoom(0);

            if (zones != null)
            {
                for (var i = 0; i < zones.Length; i++)
                {
                    if (!zones[i].ContainsPoint(zonePoint.transform.position))
                        continue;

                    zonePoint.SetRoom(i + 1);
                    break;
                }
            }

            nodes.Add(
                new Node
                {
                    point = zonePoint,
                    connexions = connexions.ToArray()
                });
        }
    }

    private void OnDrawGizmos()
    {
        if (nodes == null)
            return;
        
        for (var i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].point.GetRoom() != 0)
            {
                var random = new Random(nodes[i].point.GetRoom());
                var hue = (random.Next() & 0xFF) / 255f;
                var value = ((random.Next() & 0xFF) / 255f) * .75f + .25f;
                Gizmos.color = Color.HSVToRGB(hue, 1, value);
            }
            else
            {
                Gizmos.color = Color.white;
            }
            
            Gizmos.DrawSphere(nodes[i].point.transform.position, .5f);
            Gizmos.color = Color.white;
            foreach (var connexion in nodes[i].connexions)
            {
                if (connexion < i)
                    continue;
                
                Gizmos.DrawLine(nodes[i].point.transform.position, nodes[connexion].point.transform.position);
            }
        }
    }
}