using System;
using Player;
using UnityEngine;
using ZoneGraph;

namespace Monster
{
    public class MonsterNavigation : MonsterBehaviour
    {
        [SerializeField] private float refreshTime;
        private float _refreshTimer;
        
        private void Update()
        {
            if (_refreshTimer > 0)
            {
                _refreshTimer -= Time.deltaTime;
                return;
            }

            _refreshTimer = refreshTime;

            var node = ZoneGraphManager.Pathfinding.PathfindToPoint(transform.position, PlayerRoot.Position);

            if (node.id < 0)
            {
                MonsterData.TargetPoint = transform.position;
                return;
            }
            
            MonsterData.TargetPoint = ZoneGraphManager.Instance.GetNodePosition(node);
        }
    }
}