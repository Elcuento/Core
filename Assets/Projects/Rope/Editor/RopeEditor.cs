using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JTI.Projects.Rope2D
{
    [CustomEditor(typeof(Rope))]
    public class RopeEditor : UnityEditor.Editor
    {
        public Rope Rope;

        private enum EditType
        {
            Create,
            Remove
        }

        private EditType _mode;

        private void OnSceneGUI()
        {
            Rope.EditorUpdate();
            Input();
            Draw();
        }

        public int GetClosesPointIndex(Vector3 transformPosition)
        {
            return Rope.Segments.IndexOf(Rope.Segments.OrderBy(x => Vector3.Distance(x.transform.position, transformPosition)).FirstOrDefault());
        }

        void Input()
        {
            var guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1 && guiEvent.shift)
            {
                if (_mode == EditType.Create)
                {                    
                    if (Rope.Segments.Count <= 0)
                    {
                            Undo.RecordObject(Rope, "Add segment");
                            Rope.Spawn(Rope.transform.position);
                            EditorUtility.SetDirty(Rope);
                            for (var index = 0; index < Rope.Segments.Count; index++)
                            {
                                var ropeSegment = Rope.Segments[index];
                                EditorUtility.SetDirty(ropeSegment);
                            }

                      
                    }
                    else
                    {
                       
                            Undo.RecordObject(Rope, "Add segment");
                            var maxPos = (Vector2) Rope.Segments[Rope.Segments.Count - 1].transform.position;
                            Rope.Spawn(maxPos + (mousePos - maxPos).normalized * Rope.SegmentSpacing);
                            EditorUtility.SetDirty(Rope);
                            for (var index = 0; index < Rope.Segments.Count; index++)
                            {
                                var ropeSegment = Rope.Segments[index];
                                EditorUtility.SetDirty(ropeSegment);
                            }

                    }
                }
                else
                {

                    Undo.RecordObject(Rope, "Remove segment");

                    if (Rope.Segments.Count > 0)
                    {
                        //  var point = GetClosesPointIndex(mousePos);
                        Rope.DestroySegmentAt(Rope.Segments.Count-1); //.RemoveAt(point);
                        EditorUtility.SetDirty(Rope);
                        for (var index = 0; index < Rope.Segments.Count; index++)
                        {
                            var ropeSegment = Rope.Segments[index];
                            EditorUtility.SetDirty(ropeSegment);
                        }
                    }
                }
            }

        }


        void Draw()
        {

            for (int i = 0; i < Rope.Segments.Count; i++)
            {
                Handles.color = Color.black;

                if (i + 1 < Rope.Segments.Count)
                {
                    Handles.DrawLine(Rope.Segments[i].transform.position, Rope.Segments[i+1].transform.position);
                }
            }

            Handles.color = Color.red;

            for (int i = 0; i < Rope.Segments.Count; i++)
            {
                var fmh_108_94_638248751978131520 = Quaternion.identity; Vector2 newPos = Handles.FreeMoveHandle(Rope.Segments[i].transform.position, .1f,
                    Vector2.zero, Handles.CylinderHandleCap);

                if ((Vector2)Rope.Segments[i].transform.position != newPos)
                {
                    Undo.RecordObject(Rope, "Move point");
                    Rope.Segments[i].transform.position = newPos;
                    EditorUtility.SetDirty(Rope);
                    foreach (var ropeSegment in Rope.Segments)
                    {
                        EditorUtility.SetDirty(ropeSegment);
                    }
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            GUILayout.Label(" [Shift + Mouse left] Add/Remove segment");
            
            if (GUILayout.Button("Mode : " + _mode))
            {
                Undo.RecordObject(Rope, "Set Mode");
                _mode = _mode == EditType.Create ? EditType.Remove : EditType.Create;
                EditorUtility.SetDirty(this);
            }

            if (GUILayout.Button("ReCalculate distance"))
            {
                Undo.RecordObject(Rope, "Fix Points");
                Rope.ReCalculateDistance();
                EditorUtility.SetDirty(Rope);
                foreach (var ropeSegment in Rope.Segments)
                {
                    EditorUtility.SetDirty(ropeSegment);
                }
            }

            if (GUILayout.Button("Make Straight X"))
            {
                Undo.RecordObject(Rope, "Make Straight X");
                Rope.AligX();
                EditorUtility.SetDirty(Rope);
                foreach (var ropeSegment in Rope.Segments)
                {
                    EditorUtility.SetDirty(ropeSegment);
                }

                Draw();
            }
            if (GUILayout.Button("Make Straight Y"))
            {
                Undo.RecordObject(Rope, "Make Straight Y");
                Rope.AligY();
                EditorUtility.SetDirty(Rope);
                foreach (var ropeSegment in Rope.Segments)
                {
                    EditorUtility.SetDirty(ropeSegment);
                }
                Draw();
            }

            if (GUILayout.Button("Re Initialize"))
            {
                Rope.InitRope();
                EditorUtility.SetDirty(Rope);
                foreach (var ropeSegment in Rope.Segments)
                {
                    EditorUtility.SetDirty(ropeSegment);
                }
            }
            if (GUILayout.Button("Clear"))
            {
                Undo.RecordObject(Rope, "Clear Rope");
                Rope.Clear();
                EditorUtility.SetDirty(Rope);
            }
        }

        void OnEnable()
        {
            Rope = (Rope)target;
        }
    }
}
