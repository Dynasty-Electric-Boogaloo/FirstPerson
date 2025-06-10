using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Video;

public class CinematicSystem : MonoBehaviour
{
    public static CinematicSystem instance;
    [SerializeField] private List<VideoClip> clips;
    private VideoPlayer _videoPlayer;
    private void Awake()
    {
        if (instance == null) 
            instance = this;

        _videoPlayer = GetComponent<VideoPlayer>()
            ? GetComponent<VideoPlayer>()
            : gameObject.AddComponent<VideoPlayer>();
    }
    
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void PlayCinematic(int index = 0)
    {
        if(index > instance.clips.Count)
            return;

        instance._videoPlayer.clip = instance.clips[index];
        instance._videoPlayer.Play();
    }
}
