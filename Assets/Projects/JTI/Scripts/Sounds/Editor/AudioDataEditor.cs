using JTI.Scripts.Localization.Data;
using UnityEditor;
using UnityEngine;

namespace JTI.Scripts.Localization
{
    [CustomEditor(typeof(AudioData))]
    public class AudioDataEditor : Editor
    {
        public AudioData Target;

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
            Target = (AudioData)target;
        }
    }

}
