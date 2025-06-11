using Player;
using UnityEngine;

public class PlayerFeedback : PlayerBehaviour
{
    [SerializeField] private ParticleSystem getEnergy;
    [SerializeField] private ParticleSystem setEnergy;

    public void GetEnergy()
    {
        if(getEnergy)
            getEnergy.Play();
        if(setEnergy)
            setEnergy.Play();
    }
}
