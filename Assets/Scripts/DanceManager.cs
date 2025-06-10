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
    [SerializeField] private float tolerance = 0.5f;
    
    private MimicDestructionQte _currentMimicQTE;
    private PlayerDance _playerDance;
    private bool qteIsPlaying;
    private float timer;
    private int currentIndex;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
    
    public bool GetIsInQte => qteIsPlaying;

    private void Update()
    {
        if(!qteIsPlaying)
            return;
        
        timer += Time.deltaTime;
        
        if(!Input.GetKeyDown(KeyCode.Space))
            return;
        
        SetNextQte( timer > _currentMimicQTE.notes[currentIndex] - tolerance && timer < _currentMimicQTE.notes[currentIndex] + tolerance);
    }

    public static void StartQte(PlayerDance player, bool isMimic = true)
    {
        if(!_instance)
            return;
        
        _instance._playerDance = player;
        _instance._currentMimicQTE = isMimic
            ? _instance.qteMimic[Random.Range(0, _instance.qteMimic.Count)]
            : _instance.qteMonstre[Random.Range(0, _instance.qteMonstre.Count)];
        
        _instance.PlayQte( 0);
    }

    private void PlayQte(int index)
    {
       currentIndex = index;
       qteIsPlaying = true;
       QteUiPanel.SetQte(_currentMimicQTE.notes[index]);
    }

    private void SetNextQte( bool isWon)
    {
        timer = 0;
        if (!isWon)
        {
            _playerDance.SetQteResult(false);
            _instance.qteIsPlaying = false;
            return;
        }
        if(currentIndex < _currentMimicQTE.notes.Count-1)
            PlayQte(currentIndex + 1);
        else
        {
            _playerDance.SetQteResult(true);
            _instance.qteIsPlaying = false;
        }
    }
    
    [Serializable]
    private struct MimicDestructionQte
    {
        public List<float> notes;
    }
}
