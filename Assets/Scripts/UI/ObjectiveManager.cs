using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
   public static ObjectiveManager instance;
   public List<ObjectivePickUp> objectifs = new List<ObjectivePickUp>();

   private void Awake()
   {
      if (instance == null) 
         instance = this;
   }

   private void Start()
   {
      UpdateObjective();
   }

   public static void UpdateObjective()
   {
      foreach (var objective in instance.objectifs)
      {
         objective.gameObject.SetActive(PlayerRoot.CurrentIndex+1 == objective.indexObjective);
      }
   }
}
