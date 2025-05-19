using System;
using Player;
using UnityEngine;

public class PlayerMusicBox : PlayerBehaviour
{
    [SerializeField] private float minimalDistance;
    [SerializeField] private MusicBoxObject musicBoxObject;
    private bool _isOnDisplay;

    private void Awake()
    {
        musicBoxObject.gameObject.SetActive(_isOnDisplay);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isOnDisplay = !_isOnDisplay;
            musicBoxObject.gameObject.SetActive(_isOnDisplay);
        }
        if(_isOnDisplay)
            print(Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition(), transform.position) < minimalDistance);
    }
}
