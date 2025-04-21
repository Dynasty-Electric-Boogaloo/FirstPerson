using UnityEngine;

namespace Monster
{
    public abstract class MonsterBehaviour : MonoBehaviour
    {
        protected MonsterData MonsterData;

        public void Setup(MonsterData monsterData)
        {
            MonsterData = monsterData;
        }
    }
}