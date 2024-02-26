using UnityEditor;
using UnityEngine;

namespace Assets.Game.Rope.Scripts
{
    public class RopeSettings : MonoBehaviour
    {
      //  public Assets.Rope.Rope RopePrefab;

/*#if UNITY_EDITOR
        private void OnValidate()
        {
            var path = AssetDatabase.GetAssetPath(RopePrefab);

            if (string.IsNullOrEmpty(path)) return;

            var p = AssetDatabase.LoadAssetAtPath<Assets.Rope.Rope>(path);

            RopePrefab = p;
        }
#endif*/

    }
}
