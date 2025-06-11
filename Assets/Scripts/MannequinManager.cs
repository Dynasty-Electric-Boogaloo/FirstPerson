using UnityEngine;

public class MannequinManager : MonoBehaviour
{
   public static MannequinManager instance;
   
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
}
