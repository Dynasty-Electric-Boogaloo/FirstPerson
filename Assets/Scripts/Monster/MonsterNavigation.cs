using System;
using Heatmap;
using Player;
using UnityEngine;
using ZoneGraph;

namespace Monster
{
    public class MonsterNavigation : MonsterBehaviour
    {
        private static MonsterNavigation _instance;
        [SerializeField] private float refreshTime;
        [SerializeField] private float distanceWeight;
        [SerializeField] private float heatWeight;
        [SerializeField] private float detectionMaxDistance;
        [SerializeField] private float idleDetectionDot;
        [SerializeField] private float chasingDetectionDot;
        [SerializeField] private float detectionRayOffset;
        [SerializeField] private LayerMask visionMask;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float chaseTime;
        private float _refreshTimer;
        private NodeId _targetNode;
        private RoomId _baseRoom;
        private RaycastHit _playerHit;
        private float _chaseTimer;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        private void Start()
        {
            _baseRoom = ZoneGraphManager.Pathfinding.GetPointRoom(transform.position);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void Update()
        {
            MonsterData.stateTime += Time.deltaTime;
            
            var diff = MonsterData.targetPoint - transform.position;
            diff.y = 0;
            
            if (_refreshTimer > 0 && diff.magnitude > 0.1f)
            {
                _refreshTimer -= Time.deltaTime;
                return;
            }

            _refreshTimer = refreshTime;

            _targetNode = EvaluateTargetNode();

            if (_targetNode.id < 0)
            {
                MonsterData.targetPoint = transform.position;
                return;
            }
            
            MonsterData.targetPoint = ZoneGraphManager.Instance.GetNodePosition(_targetNode);
        }

        public static void Alert(Vector3 point)
        {
            if (_instance == null)
                return;
            
            var room = ZoneGraphManager.Pathfinding.GetPointRoom(point);
            HeatmapManager.GetRoomHeatmap(room, ref _instance.MonsterData.Heatmap);
            HeatmapManager.StartRecording(room);
            _instance._refreshTimer = 0;
            _instance.MonsterData.searching = true;
        }

        private NodeId EvaluateTargetNode()
        {
            var shouldChase = CanChase();

            HandleStateChange(MonsterData.chasing, shouldChase || _chaseTimer > 0);

            MonsterData.chasing = shouldChase || _chaseTimer > 0;
            UpdateChaseTimer(shouldChase);

            return MonsterData.chasing ? EvaluateChasingTargetNode() : EvaluateSearchingTargetNode();
        }

        private bool CanChase()
        {
            var diff = PlayerRoot.Position - transform.position;
            var rayVector = diff.normalized;

            if (diff.magnitude > detectionMaxDistance)
                return false;
            
            diff.y = 0;
            diff.Normalize();
            
            if (Vector3.Dot(transform.forward, diff) < (MonsterData.chasing ? chasingDetectionDot : idleDetectionDot))
                return false;
            
            var ray = new Ray(transform.position + Vector3.up * detectionRayOffset, rayVector);
            var hit = Physics.Raycast(ray, out _playerHit, detectionMaxDistance, visionMask);

            if (hit)
                return ((1 << _playerHit.transform.gameObject.layer) & playerMask) != 0;
            
            return false;
        }

        private void HandleStateChange(bool previousChase, bool newChase)
        {
            var stateValue = (previousChase ? 1 : 0) + (newChase ? 2 : 0);
            var room = ZoneGraphManager.Pathfinding.GetPointRoom(PlayerRoot.Position);

            switch (stateValue)
            {
                case 1:
                    HeatmapManager.StartRecording(room);
                    break;
                case 2:
                    Alert(PlayerRoot.Position);
                    break;
            }
        }

        private void UpdateChaseTimer(bool shouldChase)
        {
            if (_chaseTimer > 0)
                _chaseTimer -= Time.deltaTime;

            if (shouldChase)
                _chaseTimer = chaseTime;
        }
        
        private NodeId EvaluateChasingTargetNode()
        {
            return ZoneGraphManager.Pathfinding.PathfindToPoint(transform.position, PlayerRoot.Position);
        }

        private NodeId EvaluateSearchingTargetNode()
        {
            var nodes = ZoneGraphManager.Instance.Nodes;

            if (_targetNode.id >= 0)
            {
                var diff = nodes[_targetNode.id].Position - transform.position;
                diff.y = 0;
                
                if (diff.magnitude < 0.1f)
                    MonsterData.Heatmap.Data.Remove(_targetNode);
            }

            if (MonsterData.Heatmap.Data.Count == 0)
            {
                HeatmapManager.GetRoomHeatmap(_baseRoom, ref MonsterData.Heatmap);
                HeatmapManager.StopRecording();
                MonsterData.searching = false;
            }

            var bestNode = new NodeId(-1);
            var bestScore = float.PositiveInfinity;

            foreach (var node in MonsterData.Heatmap.Data.Keys)
            {
                var distance = Vector3.Distance(nodes[node.id].Position, transform.position);
                var heat = 1 - MonsterData.Heatmap.Data[node];

                var score = distance * distanceWeight + heat * heatWeight;

                if (score >= bestScore)
                    continue;

                bestNode = node;
                bestScore = score;
            }

            if (bestNode.id < 0)
                return bestNode;

            return ZoneGraphManager.Pathfinding.PathfindToPoint(transform.position, nodes[bestNode.id].Position);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
                return;

            Gizmos.color = _targetNode.id > 0 ? Color.white : Color.red;
            Gizmos.DrawSphere(MonsterData.targetPoint, 1);
        }
    }
}