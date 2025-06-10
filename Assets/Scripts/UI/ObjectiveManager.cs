using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
   public static ObjectiveManager instance;
   public List<ObjectivePickUp> objectifs = new List<ObjectivePickUp>();
   private List<ObjectivePickUp> _pickedUp = new List<ObjectivePickUp>();

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

   private void Start()
   {
      UpdateObjective();
   }

   public static void UpdateObjective()
   {
      if (instance == null)
         return;
      
      foreach (var objective in instance.objectifs)
         objective.gameObject.SetActive(PlayerRoot.CurrentIndex+1 == objective.indexObjective);
   }

   public static void AddToFound(ObjectivePickUp objectivePickUp)
   {
      if (!instance)
         return;
      
      instance._pickedUp.Add(objectivePickUp);
      objectivePickUp.gameObject.SetActive(false);
   }
   
   public static void RemoveToFound()
   {
      if (!instance || instance._pickedUp.Count < 1)
         return;
      
      instance._pickedUp.RemoveAt( instance._pickedUp.Count);
   }

   public static bool isInList(ObjectivePickUp pickUp) => instance && instance._pickedUp.Contains(pickUp);
}
