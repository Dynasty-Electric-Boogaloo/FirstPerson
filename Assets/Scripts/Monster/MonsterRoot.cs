using Heatmap;
using Interactables;
using Monster.Procedural;
using Player;
using UI;
using UnityEngine;
using ZoneGraph;

namespace Monster
{
    public class MonsterRoot : MonoBehaviour
    {
        private static MonsterRoot _instance;
        [SerializeField] private ProceduralHead proceduralHead;
        [SerializeField] private float hitStunTime;
        private MonsterData _monsterData;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        
        public static Vector3 GetMonsterPosition() => _instance.transform.position; 
        
        private void Awake()
        {
            if (!_instance)
                _instance = this;

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
            
        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void Update()
        {
            if (_monsterData.hitStunTimer > 0)
                _monsterData.hitStunTimer -= Time.deltaTime;
            
            proceduralHead.SetPose(_monsterData.chasing && _monsterData.watchTimer <= 0 ? "Chasing" : "Patrolling");

            if (PlayerRoot.GetIsInMannequin)
                return;

            if (!ShouldKillPlayer())
                return;
            
            PlayerRoot.Die();
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

        public bool ShouldKillPlayer()
        {
            var diff = PlayerRoot.Position - transform.position;
            var posDiff = diff;
            posDiff.y = 0;
            
            if (posDiff.magnitude < 1.5f && Mathf.Abs(diff.y) < 2)
                return true;
            
            return IsPointInMonster(PlayerRoot.Position);
        }

        public static bool IsPointInMonster(Vector3 point)
        {
            if (!_instance)
                return false;
            
            foreach (var bodyPart in _instance.proceduralHead.GetBodyParts())
            {
                var diff = point - bodyPart.transform.position;

                if (diff.magnitude < .5f)
                    return true;
            }

            return false;
        }
        
        public static void SetVisible(bool visible)
        {
            if(_instance)
                _instance.gameObject.SetActive(visible);
        }

        public static void Hit()
        {
            if (_instance)
                _instance._monsterData.hitStunTimer = _instance.hitStunTime;
        }
    }
}