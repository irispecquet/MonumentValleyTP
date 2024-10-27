using UnityEditor;

namespace Editor
{
    public static class OpenSaveDirectory
    {
        [MenuItem("Shortcut/Open Save Path")]
        private static void OpenSavePath()
        {
            System.Diagnostics.Process.Start($@"{UnityEngine.Application.persistentDataPath}/Saves");
        }
    }
}
