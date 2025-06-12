using System.Collections.Generic;
using UnityEngine;

public class MannequinManager : MonoBehaviour
{
   public static MannequinManager instance;
   private List<Mimic> _isInfected = new();
   private List<Mimic> _isSafe = new();
   
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
   
   public static void SwitchVessel(Mimic mimic)
   {
      if (!instance || !instance._isInfected.Contains(mimic)) 
         return;
      
      var randomPickMimicSafe = instance._isSafe[Random.Range(0, instance._isSafe.Count)]; 
         
      mimic.SetInfected(false);
      instance._isSafe.Add(mimic);
      instance._isInfected.Remove(mimic);

      instance._isSafe.Remove(randomPickMimicSafe);
      instance._isInfected.Add(randomPickMimicSafe);
      randomPickMimicSafe.SetInfected(true);
   }

   public static void AddToList(Mimic mimic, bool isInfected)
   {
      if (!instance)
          return;
      
      if(isInfected)
         instance._isInfected.Add(mimic);
      else
         instance._isSafe.Add(mimic);
   }
}
