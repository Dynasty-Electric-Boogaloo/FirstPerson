using System;
using TMPro;
using UnityEngine;

public class InformationManager : MonoBehaviour
{
   public static InformationManager Instance;
   [SerializeField] private TMP_Text tmpText;

   private void Awake()
   {
       if (Instance == null)
           Instance = this;

       if (Instance.tmpText)
           tmpText.text = "";
   }

   private void OnDestroy()
   {
       if (Instance == this)
           Instance = null;
   }

   public static void SetText(string text, float time)
   {
       if(!Instance || !Instance.tmpText)
           return;

       Instance.tmpText.text = text;
       Instance.Invoke(nameof(GoBack), time);
   }

   private void GoBack()
   {
       tmpText.text = "";
   }
}
