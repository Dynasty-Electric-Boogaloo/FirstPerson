using UnityEngine;
using FMODUnity;
using EventReference = FMODUnity.EventReference;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the Scene");
            return;
        }
        instance = this;
    }

    public static void PlayOneShot(EventReference sound, Vector3 worldpos)
    {
        RuntimeManager.PlayOneShot(sound, worldpos);
    }

    public static void PlayOneShot2D(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
