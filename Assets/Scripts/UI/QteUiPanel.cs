using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QteUiPanel : MonoBehaviour
{
    private static QteUiPanel _instance;
    
    [SerializeField] Image _qteTarget;
    [SerializeField] Image _qtePlayerImpulse;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);
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

        _instance._qtePlayerImpulse.transform.DOScale(0, 0);
        _instance._qtePlayerImpulse.transform.DOScale(1, timing);
    }

    public static void HideQte()
    {
        if(_instance == null)
            return;
        
        _instance._qteTarget.gameObject.SetActive(false);
        _instance._qtePlayerImpulse.gameObject.SetActive(false);
    }
    
    
}
