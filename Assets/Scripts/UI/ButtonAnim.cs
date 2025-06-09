using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnim : MonoBehaviour
{
    [SerializeField] private float enterTransitionTime = 0.5f;
    [SerializeField] private float exitTransitionTime = 1;
    [SerializeField] private Vector3 sizeToGo = new Vector3(1.25f, 1.25f, 1.25f);
    private Vector3 _defaultSize;

    private void Start()
    {
        _defaultSize = transform.lossyScale;
    }
 
    public void OnPointerEnter()
    {
        transform.DOScale(sizeToGo, enterTransitionTime).SetUpdate(true);
    }

    public void OnPointerExit()
    {
        transform.DOScale(_defaultSize, exitTransitionTime).SetUpdate(true);
    }

}
