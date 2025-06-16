using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Monster;
using Player;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
   public static ObjectiveManager instance;
   [SerializeField] private Animator animator;
   [SerializeField] private Transform endCamera;
   private List<ObjectivePickUp> _pickedUp = new List<ObjectivePickUp>();
   private static readonly int Animate = Animator.StringToHash("Animate");

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
      animator.gameObject.SetActive(false);
   }

   public static void UpdateObjective()
   {
      if (instance == null)
         return;
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
      
      instance._pickedUp.RemoveAt( instance._pickedUp.Count-1);
   }

   public static void Win()
   {
      MonsterRoot.Freeze();
      instance.animator.gameObject.SetActive(true);
      instance.animator.SetTrigger(Animate);
      if(instance.endCamera)
      {
         PlayerRoot.SetCamera(instance.endCamera, true);
         instance.StartCoroutine(nameof(GoBack));
      }
   }

   private IEnumerator GoBack()
   {
      if (!instance.endCamera)
         yield break;

      yield return new WaitForSeconds(3);
      PlayerRoot.SetCamera();
   }

   public static bool isInList(ObjectivePickUp pickUp) => instance && instance._pickedUp.Contains(pickUp);
   public static bool isLast => instance && instance._pickedUp.Count >= 3;
}
