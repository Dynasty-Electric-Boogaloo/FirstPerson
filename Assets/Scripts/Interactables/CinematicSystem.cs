using System;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicSystem : MonoBehaviour
{
    public static CinematicSystem instance;
    [SerializeField] private List<VideoClip> clips;
    [SerializeField] private RawImage image;
    [SerializeField] private Transform endCamera;
    private VideoPlayer _videoPlayer;
    private void Awake()
    {
        if (instance == null) 
            instance = this;

        _videoPlayer = GetComponent<VideoPlayer>()
            ? GetComponent<VideoPlayer>()
            : gameObject.AddComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += EndOfCinematic;
        image.gameObject.SetActive(false);
    }

    private void Start()
    {
        PlayCinematic();
    }

    private void EndOfCinematic(VideoPlayer source)
    {
        source.Stop();
        image.gameObject.SetActive(false);
        PauseManager.PauseGame(false, false, false);
        PauseManager.SetForcePause(false);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private static void PlayCinematic(int index = 0)
    {
        if(!instance || index > instance.clips.Count)
            return;

        instance._videoPlayer.clip = instance.clips[index];
        instance._videoPlayer.Play();
        PauseManager.PauseGame(true, false);
        PauseManager.SetForcePause(true);
        instance.image.gameObject.SetActive(true);
    }
    
    private static void EndGame()
    {
        if(!instance)
            return;

        instance.image.DOColor(Color.clear, 0);
        instance.image.texture = null;
        instance.image.DOColor(Color.black, 1);
    }
    
}
