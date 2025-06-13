using System;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
  [SerializeField] private Transform doorPivot;
  [SerializeField] private Vector3 eulerRotationOpen;
  [SerializeField] private float openingSpeed = 1;
  [SerializeField] private float maxDistanceBeforeClosing = 3;
  [SerializeField] private bool autoClose = true;
  private bool _isOpened; 
  
  public void ChangeState(bool opening = true)
  {
    doorPivot.transform.DORotate(opening ? eulerRotationOpen : Vector3.zero, openingSpeed);
    _isOpened = opening;
  }

  private void Update()
  {
    if(!_isOpened || !autoClose)
      return;
    
    if(Vector3.Distance(PlayerRoot.Position, transform.position) > maxDistanceBeforeClosing)
      ChangeState(false);
  }
}
