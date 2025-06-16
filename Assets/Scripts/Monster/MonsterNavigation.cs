using System;
using Heatmap;
using Player;
using UI;
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
        [SerializeField] private float watchTime;
        [SerializeField] private float watchDistance;
        [SerializeField] private LayerMask visionMask;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float chaseTime;
        [SerializeField] private AudioClip alertClip;
        [SerializeField] private Light stageLight;
        private float _refreshTimer;
        private RoomId _baseRoom;
        private RaycastHit _playerHit;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        private void Start()
        {
            _baseRoom = ZoneGraphManager.Pathfinding.GetPointRoom(transform.position);
            DanceManager.OnQteOver?.AddListener(OnQteOver);
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void Update()
        {
            var monsterRoom = ZoneGraphManager.Pathfinding.GetPointRoom(transform.position);
            var monsterPoint = ZoneGraphManager.Pathfinding.GetPointClosestNode(transform.position, monsterRoom);
            
            MonsterData.Heatmap.Data.Remove(monsterPoint);
            
            stageLight.enabled = monsterRoom == _baseRoom;
            
            MonsterData.stateTime += Time.deltaTime;
            
            if (MonsterData.watchTimer > 0)
                MonsterData.watchTimer -= Time.deltaTime;
            
            if (_refreshTimer > 0 && monsterPoint != MonsterData.targetNode)
            {
                _refreshTimer -= Time.deltaTime;
                return;
            }
            
            _refreshTimer = refreshTime;

            MonsterData.targetNode = EvaluateTargetNode();
            
            if (MonsterData.targetNode.id < 0)
                MonsterData.targetNode = ZoneGraphManager.Pathfinding.GetPointClosestNode(transform.position, ZoneGraphManager.Pathfinding.GetPointRoom(transform.position));
            
            MonsterData.targetPoint = ZoneGraphManager.Instance.GetNodePosition(MonsterData.targetNode);
        }

        public static void Alert(Vector3 point, bool silent = false)
        {
            if (_instance == null)
                return;
            
            var room = ZoneGraphManager.Pathfinding.GetPointRoom(point);
            HeatmapManager.GetRoomHeatmap(room, ref _instance.MonsterData.Heatmap);
            HeatmapManager.StartRecording(room);
            _instance._refreshTimer = 0;
            _instance.MonsterData.searching = true;
            
            if (_instance.alertClip && !silent)
                AudioSource.PlayClipAtPoint(_instance.alertClip, point, .25f);
        }
        
        private void OnQteOver(bool win)
        {
            MonsterData.watchTimer = win ? watchTime : 0;
            QteUiPanel.HideQte();
        }

        private NodeId EvaluateTargetNode()
        {
            var shouldChase = CanChase();

            HandleStateChange(MonsterData.chasing, shouldChase || MonsterData.chaseTimer > 0 || MonsterData.watchTimer > 0);

            MonsterData.chasing = shouldChase || MonsterData.chaseTimer > 0 || MonsterData.watchTimer > 0;
            UpdateChaseTimer(shouldChase);

            if (shouldChase && MonsterData.watchTimer > 0 && Vector3.Distance(transform.position, PlayerRoot.Position) < watchDistance)
                return MonsterData.targetNode;

            return MonsterData.chasing ? EvaluateChasingTargetNode() : EvaluateSearchingTargetNode();
        }

        private bool CanChase()
        {
            if (PlayerRoot.GetIsInMannequin)
                return false;
            
            var playerRoom = ZoneGraphManager.Pathfinding.GetPointRoom(PlayerRoot.Position);
            var monsterRoom = ZoneGraphManager.Pathfinding.GetPointRoom(transform.position);
            
            if (playerRoom == _baseRoom && monsterRoom == _baseRoom)
                return true;
            
            var diff = PlayerRoot.Position - transform.position;
            var rayVector = diff.normalized;

            if (diff.magnitude > detectionMaxDistance || Mathf.Abs(diff.y) > 2)
                return false;
            
            diff.y = 0;
            diff.Normalize();
            
            if (Vector3.Dot(transform.forward, diff) < (MonsterData.chasing ? chasingDetectionDot : idleDetectionDot))
                return false;
            
            var ray = new Ray(transform.position + Vector3.up * detectionRayOffset, rayVector);
            var hit = Physics.Raycast(ray, out _playerHit, detectionMaxDistance, visionMask);

            if (hit && !PlayerRoot.GetIsDancing())
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
                    MonsterData.stateTime = 0;
                    HeatmapManager.StartRecording(room);
                    UiManager.SetChaseBorder(false);
                    break;
                case 2:
                    MonsterData.stateTime = 0;
                    Alert(PlayerRoot.Position, true);
                    AudioManager.PlayOneShot(FMODEvents.GetSpotted(), transform.position);
                    if (MonsterData.watchTimer <= 0)
                        PlayerRoot.StartQte(false);
                    MonsterData.watchTimer = 10;
                    UiManager.SetChaseBorder(true);
                    break;
            }
        }

        private void UpdateChaseTimer(bool shouldChase)
        {
            if (MonsterData.chaseTimer > 0)
                MonsterData.chaseTimer -= Time.deltaTime;

            if (shouldChase)
                MonsterData.chaseTimer = chaseTime;
        }
        
        private NodeId EvaluateChasingTargetNode()
        {
            return ZoneGraphManager.Pathfinding.PathfindToPoint(transform.position, PlayerRoot.Position);
        }

        private NodeId EvaluateSearchingTargetNode()
        {
            var nodes = ZoneGraphManager.Instance.Nodes;

            if (MonsterData.Heatmap.Data.Count == 0)
            {
                HeatmapManager.GetRoomHeatmap(_baseRoom, ref MonsterData.Heatmap);
                HeatmapManager.StopRecording();
                MonsterData.searching = false;
            }

            var bestNode = new NodeId(-1);
            var bestScore = float.MaxValue;

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

            Gizmos.color = MonsterData.targetNode.id > 0 ? Color.white : Color.red;
            Gizmos.DrawSphere(MonsterData.targetPoint, 1);
        }
    }
}