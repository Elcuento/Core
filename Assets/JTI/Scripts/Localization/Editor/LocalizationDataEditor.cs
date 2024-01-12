using JTI.Scripts.Localization.Data;
using UnityEditor;
using UnityEngine;

namespace JTI.Scripts.Localization
{
    [CustomEditor(typeof(LocalizationData))]
    public class LocalizationDataEditor : Editor
    {
        public LocalizationData Target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Update"))
            {
                Target.UpdateData();
            }

        }

        void OnEnable()
        {
            Target = (LocalizationData)target;
        }
    }

}
