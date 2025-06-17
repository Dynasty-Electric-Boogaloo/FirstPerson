using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
   public class PauseManager : MonoBehaviour
   {
      public static PauseManager instance;
      [SerializeField] private GameObject pausePanel;
      private bool _pause;
      private bool _forcePause;
      private PlayerInputs _inputs;

      private void Awake()
      {
         if (instance == null)
            instance = this;
         
         if(pausePanel)
            pausePanel.SetActive(false);
         
         _inputs = new PlayerInputs(); 
         _inputs.Enable();
      }
      
      private void OnDestroy()
      {
         if (instance == this)
            instance = null;
         
         _inputs.Disable();
      }
      
      private void Update()
      {
         if(_forcePause)
            return;
         
         if(_inputs.Controls.Return.WasPressedThisFrame())
            PauseGame(!_pause);
      }
      
      public static void PauseGame(bool setPause, bool showMenu = true, bool forceLockMouse = true)
      {
         if(instance == null || !PlayerRoot.GetRedLightUnlocked)
            return;
         
         if(instance.pausePanel && showMenu)
            instance.pausePanel.SetActive(setPause);
            
         instance._pause = setPause;
         Time.timeScale = setPause ? 0f : 1f;
         Cursor.visible = !forceLockMouse || setPause;
         
         if (!forceLockMouse)
            Cursor.lockState = CursorLockMode.None;
         else
            Cursor.lockState = setPause ? CursorLockMode.None : CursorLockMode.Locked ;
      }

      public static void Quit()
      {
         Application.Quit();
      }

      public static void SetForcePause(bool setOn)
      {
         if(!instance)
            return;
         
         instance._forcePause = setOn;
      }
      
      public void CancelPause()
      {
         PauseGame(false);
      }
      
      public static bool GetPause => instance && (instance._pause || instance._forcePause);
   }
}
