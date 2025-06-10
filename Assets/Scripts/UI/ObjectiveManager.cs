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
      instance._pickedUp.Add(objectivePickUp);
      objectivePickUp.gameObject.SetActive(false);
   }
   
   public static void RemoveToFound()
   {
      instance._pickedUp.RemoveAt( instance._pickedUp.Count);
   }

   public static bool isInList(ObjectivePickUp pickUp) => instance && instance._pickedUp.Contains(pickUp);
}
