using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
   public class PauseManager : MonoBehaviour
   {
      public static PauseManager instance;
      [SerializeField] private GameObject pausePanel;
      private bool _pause;
      private PlayerInputs _inputs = new PlayerInputs();

      private void Awake()
      {
         if (instance == null)
            instance = this;
         
         if(pausePanel)
            pausePanel.SetActive(false);
         

      }
      
      private void OnDestroy()
      {
         if (instance == this)
            instance = null;
         
         _inputs.Disable();
      }

      
      private void Update()
      {
         if(_inputs.Controls.Return.WasPressedThisFrame())
            PauseGame(!instance._pause);
      }
      
      public static void PauseGame(bool setPause, bool showMenu = true)
      {
         if(instance == null)
            return;
         
         if(instance.pausePanel && showMenu)
            instance.pausePanel.SetActive(setPause);
            
         instance._pause = setPause;
         Time.timeScale = setPause ? 0f : 1f;
         Cursor.visible = setPause;
         Cursor.lockState = setPause ? CursorLockMode.None : CursorLockMode.Locked;
      }

      public static void Quit()
      {
         Application.Quit();
      }
      
      public static bool GetPause() =>instance && instance._pause;
   }
}
