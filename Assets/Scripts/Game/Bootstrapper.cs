using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            var app = Object.Instantiate(Resources.Load<GameObject>("App"));

            if (app == null)
            {
                throw new ApplicationException();
            }

            app.name = "App";
            Object.DontDestroyOnLoad(app);
        }
    }
}