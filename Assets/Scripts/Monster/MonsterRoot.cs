using UnityEngine;

namespace Monster
{
    public class MonsterRoot : MonoBehaviour
    {
        private MonsterData _monsterData;
        
        private void Awake()
        {
            _monsterData = new MonsterData
            {
                Rigidbody = GetComponent<Rigidbody>(),
            };
            
            var behaviours = GetComponents<MonsterBehaviour>();
            foreach (var behaviour in behaviours)
            {
                behaviour.Setup(_monsterData);
            }
        }
    }
}