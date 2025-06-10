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
    private PlayerInputs _inputs;
    
    private MimicDestructionQte _currentMimicQTE;
    private PlayerDance _playerDance;
    private bool _qteIsPlaying;
    private float _timer;
    private int _currentIndex;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        _inputs = new PlayerInputs(); 
        _inputs.Enable();
    }
    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
        _inputs.Disable();
    }
    
    public bool GetIsInQte => _qteIsPlaying;

    private void Update()
    {
        if(!_qteIsPlaying)
            return;
        
        _timer += Time.deltaTime;
        if (_timer > _currentMimicQTE.notes[_currentIndex] + tolerance )
            SetNextQte(false);
        
        if(!_inputs.Controls.Dance.WasPressedThisFrame())
            return;
        
        SetNextQte( _timer > _currentMimicQTE.notes[_currentIndex] - tolerance && _timer < _currentMimicQTE.notes[_currentIndex] + tolerance);
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
       _currentIndex = index;
       _qteIsPlaying = true;
       QteUiPanel.SetQte(_currentMimicQTE.notes[index]);
    }

    private void SetNextQte( bool isWon)
    {
        _timer = 0;
        if (!isWon)
        {
            _playerDance.SetQteResult(false);
            _instance._qteIsPlaying = false;
            return;
        }
        if(_currentIndex < _currentMimicQTE.notes.Count-1)
            PlayQte(_currentIndex + 1);
        else
        {
            _playerDance.SetQteResult(true);
            _instance._qteIsPlaying = false;
        }
    }
    
    [Serializable]
    private struct MimicDestructionQte
    {
        public List<float> notes;
    }
}
