using Heatmap;
using Interactables;
using Monster.Procedural;
using Player;
using UnityEngine;
using ZoneGraph;

namespace Monster
{
    public class MonsterRoot : MonoBehaviour
    {
        [SerializeField] private ProceduralHead proceduralHead;
        [SerializeField] private bool chasing;
        private MonsterData _monsterData;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        
        private void Awake()
        {
            _monsterData = new MonsterData
            {
                rigidbody = GetComponent<Rigidbody>(),
                Heatmap = new HeatmapData("Monster map")
            };
            
            var behaviours = GetComponents<MonsterBehaviour>();
            foreach (var behaviour in behaviours)
            {
                behaviour.Setup(_monsterData);
            }
            
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void Update()
        {
            proceduralHead.SetPose(chasing ? "Chasing" : "Patrolling");
            
            var diff = PlayerRoot.Position - transform.position;
            diff.y = 0;

            if (diff.magnitude > 1.5f) 
                return;
            
            PlayerRoot.ResetPosition();
            transform.position = _startPosition;
            transform.rotation = _startRotation;
            _monsterData.targetPoint = transform.position;
            _monsterData.stateTime = 0;
            _monsterData.chasing = false;
            _monsterData.searching = false;
            _monsterData.Heatmap.Data.Clear();
            _monsterData.chaseTimer = 0;
            _monsterData.targetNode = new NodeId(-1);
            InteractableManager.Restore();
        }
    }
}