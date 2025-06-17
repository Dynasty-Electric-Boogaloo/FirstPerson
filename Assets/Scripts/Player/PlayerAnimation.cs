using System;
using Player;
using UnityEngine;

public class PlayerAnimation : PlayerBehaviour
{
   [SerializeField] private Animator animator;
   private static readonly int RedLight = Animator.StringToHash("RedLight");
   private static readonly int Holding = Animator.StringToHash("Holding");
   private static readonly int Mannequin = Animator.StringToHash("Mannequin");
   private static readonly int Reload = Animator.StringToHash("Reload");
   private static readonly int Danse = Animator.StringToHash("Danse");
   private static readonly int QteMimic = Animator.StringToHash("QTEMimic");
   private static readonly int Switching = Animator.StringToHash("Switching");
   private static readonly int MusicBox = Animator.StringToHash("MusicBoxOut");

   private void Update()
   {
      animator.SetBool(RedLight, PlayerData.IsSpecialLight);
      animator.SetBool(Holding, PlayerData.Holding);
      animator.SetBool(MusicBox, PlayerData.MusicBoxIsOut);
      animator.SetBool(Mannequin, PlayerData.IsInMannequin);
      animator.SetBool(Reload, PlayerData.Reloading);
      animator.SetBool(Danse, PlayerData.Dancing);
      animator.SetBool(QteMimic, PlayerData.Dancing);
   }

   public void SetSwitch()
   {
      animator.SetTrigger(Switching);
   }
}
