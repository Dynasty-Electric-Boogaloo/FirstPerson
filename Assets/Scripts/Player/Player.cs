using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform cameraHolder;
        private PlayerData _playerData;
        
        private void Awake()
        {
            _playerData = new PlayerData
            {
                PlayerInputs = new PlayerInputs(),
                Rigidbody = GetComponent<Rigidbody>(),
                CameraHolder = cameraHolder
            };
            _playerData.PlayerInputs.Enable();
            
            var behaviours = GetComponents<PlayerBehaviour>();
            foreach (var behaviour in behaviours)
            {
                behaviour.Setup(_playerData);
            }
        }
    }
}