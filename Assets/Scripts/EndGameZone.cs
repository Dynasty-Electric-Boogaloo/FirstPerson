using System;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerRoot>(out _))
            SceneManager.LoadScene("Win_Logique");
    }
}
