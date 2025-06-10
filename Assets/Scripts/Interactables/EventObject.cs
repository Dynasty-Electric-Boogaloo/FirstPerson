using Player;
using UnityEngine;

public class EventObject : MonoBehaviour
{
   public void DoEvent()
   {
      PlayerRoot.SetRedLight(true);
   }
}
