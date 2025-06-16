using System;
using System.Dynamic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QteUiPanel : MonoBehaviour
{
    private static QteUiPanel _instance;
    
    [SerializeField] Image _qteTarget;
    [SerializeField] Image _qtePlayerImpulse;
    [SerializeField] private Color winColor;
    [SerializeField] private Color looseColor;

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

    private void Start()
    {
        HideQte();
    }

    public static void SetQte(float timing)
    {
        if(_instance == null)
            return;
        
        _instance._qteTarget.gameObject.SetActive(true);
        _instance._qtePlayerImpulse.gameObject.SetActive(true);
        
        _instance._qteTarget.color = Color.white;
        _instance._qtePlayerImpulse.color = Color.white;

        _instance._qtePlayerImpulse.transform.DOScale(0, 0);
        _instance._qtePlayerImpulse.transform.DOScale(1, timing);
    }

    public static void SetResult(bool win)
    {
        _instance._qteTarget.color = win ? _instance.winColor : _instance.looseColor;
        _instance._qtePlayerImpulse.color = win ? _instance.winColor : _instance.looseColor;

        _instance.Invoke(nameof(_HideQte), .5f);
    }

    public void _HideQte()
    {
        HideQte();
    }
    
    public static void HideQte()
    {
        if(_instance == null)
            return;
        
        _instance._qteTarget.gameObject.SetActive(false);
        _instance._qtePlayerImpulse.gameObject.SetActive(false);
    }
}
