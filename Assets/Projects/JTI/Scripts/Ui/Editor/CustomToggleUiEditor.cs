
using UnityEditor;
using UnityEditor.UI;


[CustomEditor(typeof(CustomToggleUi), true)]
[CanEditMultipleObjects]
public class CustomToggleUiEditor : ToggleEditor
{
    SerializedProperty Enable;
    SerializedProperty Disable;
    SerializedProperty OnValueChangeOnEvent;

    protected override void OnEnable()
    {
        base.OnEnable();
        Enable = serializedObject.FindProperty("Enable");
        Disable = serializedObject.FindProperty("Disable");
        OnValueChangeOnEvent = serializedObject.FindProperty("OnValueChangeOnEvent");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        EditorGUILayout.PropertyField(Enable);
        EditorGUILayout.PropertyField(Disable);
        EditorGUILayout.PropertyField(OnValueChangeOnEvent);
        serializedObject.ApplyModifiedProperties();


    }
}
