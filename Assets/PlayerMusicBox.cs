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
        if(_isOnDisplay && Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition(), transform.position) < minimalDistance)
        {
            print(Vector3.Distance(Monster.MonsterRoot.GetMonsterPosition(), transform.position) < minimalDistance);
            musicBoxObject.ballerina.transform.LookAt(Monster.MonsterRoot.GetMonsterPosition());
            var rotation = musicBoxObject.ballerina.transform.rotation.eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            musicBoxObject.ballerina.transform.eulerAngles = rotation;
            
        }
    }
}
