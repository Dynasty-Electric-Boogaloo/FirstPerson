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
        private NodeId _targetNode;
        
        private void Update()
        {
            var diff = MonsterData.TargetPoint - transform.position;
            diff.y = 0;
            if (diff.sqrMagnitude < .01f)
            {
                _refreshTimer = 0;
            }
            
            if (_refreshTimer > 0)
            {
                _refreshTimer -= Time.deltaTime;
                return;
            }

            _refreshTimer = refreshTime;

            _targetNode = ZoneGraphManager.Pathfinding.PathfindToPoint(transform.position, PlayerRoot.Position);

            if (_targetNode.id < 0)
            {
                MonsterData.TargetPoint = transform.position;
                return;
            }
            
            MonsterData.TargetPoint = ZoneGraphManager.Instance.GetNodePosition(_targetNode);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            Gizmos.color = _targetNode.id > 0 ? Color.white : Color.red;
            Gizmos.DrawSphere(MonsterData.TargetPoint, 1);
        }
    }
}