using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance;
        [SerializeField] private SceneLoaderConfig config;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            
            #if UNITY_EDITOR
            config.ComputeBuildList();
            
            var index = GetSceneGroupIndexByBuildIndex(SceneManager.GetActiveScene().buildIndex);
            
            if (index < 0)
                return;
            
            LoadSceneGroup(index, LoadSceneMode.Single);
            #else
            LoadSceneGroup(0, LoadSceneMode.Single);
            #endif
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private int GetSceneGroupIndexByBuildIndex(int buildIndex)
        {
            for (var i = 0; i < config.sceneIds.Length; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (buildIndex == config.sceneIds[i].ids[j])
                        return i;
                }
            }

            return -1;
        }

        public static void LoadSceneGroup(int index, LoadSceneMode mode)
        {
            if (_instance == null)
            {
                Debug.LogError("No SceneLoader instance exists!");
                return;
            }

            if (index < 0 || index >= _instance.config.sceneIds.Length)
            {
                Debug.LogError("Scene group index out of bounds!");
                return;
            }

            var sceneId = _instance.config.sceneIds[index];

            if (sceneId.ids[0] == -1)
            {
                Debug.Log("Scene group is empty! Cannot load anything!");
                return;
            }

            SceneManager.LoadScene(sceneId.ids[0], mode);

            for (var i = 1; i < 3; i++)
            {
                if (sceneId.ids[i] < 0)
                    return;

                if (SceneManager.GetSceneByBuildIndex(sceneId.ids[i]).isLoaded)
                    continue;
                
                SceneManager.LoadScene(sceneId.ids[i], LoadSceneMode.Additive);
            }
        }

        public static async Task LoadSceneGroupAsync(int index, LoadSceneMode mode)
        {
            if (_instance == null)
            {
                Debug.LogError("No SceneLoader instance exists!");
                return;
            }

            if (index < 0 || index >= _instance.config.sceneIds.Length)
            {
                Debug.LogError("Scene group index out of bounds!");
                return;
            }

            var sceneId = _instance.config.sceneIds[index];

            if (sceneId.ids[0] == -1)
            {
                Debug.Log("Scene group is empty! Cannot load anything!");
                return;
            }

            var asyncOperations = new List<AsyncOperation>(3);
            asyncOperations.Add(SceneManager.LoadSceneAsync(sceneId.ids[0], mode));

            for (var i = 1; i < 3; i++)
            {
                if (sceneId.ids[i] < 0)
                    return;

                if (SceneManager.GetSceneByBuildIndex(sceneId.ids[i]).isLoaded)
                    continue;
                
                asyncOperations.Add(SceneManager.LoadSceneAsync(sceneId.ids[i], LoadSceneMode.Additive));
            }

            var finished = false;
                
            while (!finished)
            {
                finished = true;
                    
                foreach (var asyncOperation in asyncOperations)
                {
                    if (asyncOperation.isDone)
                        continue;
                    finished = false;
                }

                await Task.Yield();
            }
        }

        public static async Task UnloadSceneGroupAsync(int index)
        {
            if (_instance == null)
            {
                Debug.LogError("No SceneLoader instance exists!");
                return;
            }

            if (index < 0 || index >= _instance.config.sceneIds.Length)
            {
                Debug.LogError("Scene group index out of bounds!");
                return;
            }

            var sceneId = _instance.config.sceneIds[index];

            if (sceneId.ids[0] == -1)
            {
                return;
            }

            var asyncOperations = new List<AsyncOperation>(3);
            asyncOperations.Add(SceneManager.UnloadSceneAsync(sceneId.ids[0]));

            for (var i = 1; i < 3; i++)
            {
                if (sceneId.ids[i] < 0)
                    return;
                
                asyncOperations.Add(SceneManager.UnloadSceneAsync(sceneId.ids[i]));
            }
            
            var finished = false;
                
            while (!finished)
            {
                finished = true;
                    
                foreach (var asyncOperation in asyncOperations)
                {
                    if (asyncOperation.isDone)
                        continue;
                    finished = false;
                }

                await Task.Yield();
            }
        }
    }
}