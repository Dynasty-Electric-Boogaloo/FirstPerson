using UnityEngine;

namespace Player
{
    public abstract class PlayerBehaviour : MonoBehaviour
    {
        protected PlayerData PlayerData;

        public void Setup(PlayerData playerData)
        {
            PlayerData = playerData;
        }
    }
}