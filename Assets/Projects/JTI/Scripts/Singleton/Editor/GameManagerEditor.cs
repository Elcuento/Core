using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JTI.Scripts
{
    public class GameMasterEditor : Editor
    {
      /*  [MenuItem("Utils/Settings")]
        public static void OpenSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(Settings)}");
            if (guids.Length > 1)
                Debug.LogErrorFormat("[PlotBuildTools] Found more than 1 Build Properties: {0}. Using first one!",
                    guids.Length);

            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Settings>(path);
            }
            else
            {
                Debug.LogWarning("Couldn't find Build Settings, please create one!");
            }
        }
        */
        [MenuItem("JTI/Utils/Clear saves")]
        public static void Clear()
        {
            PlayerPrefs.DeleteAll();

            var di = new DirectoryInfo($"{Application.persistentDataPath}");

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }

}
