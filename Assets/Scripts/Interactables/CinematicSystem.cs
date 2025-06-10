using System.Collections.Generic;
using UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicSystem : MonoBehaviour
{
    public static CinematicSystem instance;
    [SerializeField] private List<VideoClip> clips;
    [SerializeField] private RawImage image;
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

    private void EndOfCinematic(VideoPlayer source)
    {
        source.Stop();
        image.gameObject.SetActive(false);
        PauseManager.PauseGame(false, false);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    [ContextMenu("Testing cinematique")]
    private void Testing()
    {
        PlayCinematic();
    }

    private static void PlayCinematic(int index = 0)
    {
        if(index > instance.clips.Count)
            return;

        instance._videoPlayer.clip = instance.clips[index];
        instance._videoPlayer.Play();
        PauseManager.PauseGame(true, false);
        instance.image.gameObject.SetActive(true);
    }
}
