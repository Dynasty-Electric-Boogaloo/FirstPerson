using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class DanceManager : MonoBehaviour
{
    private static DanceManager _instance;
    [SerializeField] private List<MimicDestructionQte> qteMimic = new List<MimicDestructionQte>();
    [SerializeField] private List<MimicDestructionQte> qteMonstre = new List<MimicDestructionQte>();
    private MimicDestructionQte _currentMimicQTE;
    private PlayerDance _playerDance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    public static void StartQte(PlayerDance player, bool isMimic = true)
    {
        if(!_instance)
            return;
        
        _instance._playerDance = player;
        _instance.PlayQte(
            isMimic
                ? _instance.qteMimic[Random.Range(0, _instance.qteMimic.Count)]
                : _instance.qteMonstre[Random.Range(0, _instance.qteMonstre.Count)], 0);
    }

    private void PlayQte(MimicDestructionQte qte, int index)
    {
       print( qte.notes[index]);

       if (false)
       {
           _playerDance.SetQteResult(false);
           return;
       }
       
       if(index < qte.notes.Count-1)
           PlayQte(qte, index + 1);
       else
           _playerDance.SetQteResult(true);
    }
    
    [Serializable]
    private struct MimicDestructionQte
    {
        public List<float> notes;
    }
}
