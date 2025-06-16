using System;
using Player;
using UnityEngine;

public class EndGameZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<PlayerRoot>(out _))
            CinematicSystem.EndGame();
    }
}
