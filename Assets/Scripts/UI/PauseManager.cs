using UnityEngine;

namespace UI
{
   public class PauseManager : MonoBehaviour
   {
      public static PauseManager instance;
      [SerializeField] private GameObject pausePanel;
      private bool _pause;

      private void Awake()
      {
         if (instance == null)
            instance = this;
         
         if(pausePanel)
            pausePanel.SetActive(false);
      }
      
      private void Update()
      {
         if (!Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.P)) 
            return;
         
         PauseGame(!_pause);
         InspectSystem.Hide();
      }
      
      public static void PauseGame(bool setPause, bool showMenu = true)
      {
         if(instance &&  instance.pausePanel && showMenu)
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
      
      public static bool GetPause() => instance._pause;
   }
}
