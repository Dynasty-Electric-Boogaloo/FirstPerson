using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MimicManager : MonoBehaviour
{
   public static MimicManager instance;
   [SerializeField] private float timer = 120;
   [SerializeField] private float distanceMax = 5;
   [SerializeField] private float angleMax = 0.9f;
   private List<Mimic> _isInfectedMannequin = new();
   private List<Mimic> _isSafeMannequin = new();
   
   private List<Mimic> _isMimic = new();
   private float _time;
   
   private void Awake()
   {
      if (instance == null)
         instance = this;
   }
      
   private void OnDestroy()
   {
      if (instance == this)
         instance = null;
   }

   private void Update()
   {
      _time += Time.deltaTime;

      if (_time < timer) 
         return;
      
      _time = 0;
      foreach (var mimic in _isMimic)
         Restore(mimic);
   }

   private void Restore(Mimic mimic)
   {
      if(Vector3.Distance(mimic.transform.position, PlayerRoot.Position) > distanceMax || Vector3.Dot(mimic.transform.position.normalized, PlayerRoot.Position.normalized) < angleMax)
         mimic.gameObject.SetActive(true);
   }

   public static void SwitchVessel(Mimic mimic)
   {
      if (!instance || !instance._isInfectedMannequin.Contains(mimic)) 
         return;
      
      if(instance._isSafeMannequin.Count > 0)
      {
         var randomPickMimicSafe = instance._isSafeMannequin[Random.Range(0, instance._isSafeMannequin.Count)];
         instance._isSafeMannequin.Remove(randomPickMimicSafe);
         instance._isInfectedMannequin.Add(randomPickMimicSafe);
         randomPickMimicSafe.SetInfected(true);
      }
         
      mimic.SetInfected(false);
      instance._isSafeMannequin.Add(mimic);
      instance._isInfectedMannequin.Remove(mimic);
      
   }

   public static void AddToList(Mimic mimic, bool isInfected, bool isMannequin = true)
   {
      if (!instance)
          return;
      
      if(!isMannequin)
      {
         instance._isMimic.Add(mimic);
         return;
      }
      
      if(isInfected)
         instance._isInfectedMannequin.Add(mimic);
      else
         instance._isSafeMannequin.Add(mimic);
   }
}
