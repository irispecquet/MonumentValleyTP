using UnityEngine;

namespace Lib
{
    public static class SystemInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            Object systemPrefab = Resources.Load("System");
            Object.DontDestroyOnLoad(Object.Instantiate(systemPrefab));
        }
    }
}
