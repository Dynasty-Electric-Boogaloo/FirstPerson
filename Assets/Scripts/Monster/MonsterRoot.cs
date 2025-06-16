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
        private MonsterBehaviour[] _behaviours;
        
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
            
            _behaviours = GetComponents<MonsterBehaviour>();
            foreach (var behaviour in _behaviours)
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

            Die();
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

        public static void Die()
        {
            if (!_instance)
                return;
            
            PlayerRoot.Die();
            _instance.transform.position = _instance._startPosition;
            _instance.transform.rotation = _instance._startRotation;
            ResetState();
            UiManager.SetChaseBorder(false);
            InteractableManager.Restore();
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

        public static void ResetState()
        {
            if (!_instance)
                return;
            
            _instance._monsterData.targetPoint = _instance.transform.position;
            _instance._monsterData.stateTime = 0;
            _instance._monsterData.chasing = false;
            _instance._monsterData.searching = false;
            _instance._monsterData.Heatmap.Data.Clear();
            _instance._monsterData.chaseTimer = 0;
            _instance._monsterData.watchTimer = 0;
            _instance._monsterData.targetNode = new NodeId(-1);
        }

        public static void Hit()
        {
            if (_instance)
                _instance._monsterData.hitStunTimer = _instance.hitStunTime;
        }

        public static void Freeze()
        {
            if (!_instance)
                return;
            
            foreach (var behaviour in _instance._behaviours)
            {
                behaviour.enabled = false;
            }

            _instance._monsterData.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _instance.enabled = false;
        }
    }
}