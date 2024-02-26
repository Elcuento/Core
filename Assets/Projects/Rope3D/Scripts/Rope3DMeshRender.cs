using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JTI.Projects.Rope3D
{
    [ExecuteInEditMode]
    public class Rope3DMeshRender : MonoBehaviour
    {
        public Action OnUpdateEvent;

        [System.Serializable]
        public class RopeSections
        {
            public List<Vector2> vertices;

            public int Segments
            {
                get { return vertices.Count - 1; }
            }

            public RopeSections()
            {
                vertices = new List<Vector2>();
                CirclePreset(8);
            }

            private void CirclePreset(int segments)
            {
                vertices.Clear();

                for (int j = 0; j <= segments; ++j)
                {
                    float angle = 2 * Mathf.PI / segments * j;
                    vertices.Add(Mathf.Cos(angle) * Vector2.right + Mathf.Sin(angle) * Vector2.up);
                }
            }

        }

        private class CurveFrame
        {

            public Vector3 position = Vector3.zero;
            public Vector3 tangent = Vector3.forward;
            public Vector3 normal = Vector3.up;
            public Vector3 binormal = Vector3.left;

            public void Reset()
            {
                position = Vector3.zero;
                tangent = Vector3.forward;
                normal = Vector3.up;
                binormal = Vector3.left;
            }

            public void SetTwist(float twist)
            {
                Quaternion twistQ = Quaternion.AngleAxis(twist, tangent);
                normal = twistQ * normal;
                binormal = twistQ * binormal;
            }

            public void SetTwistAndTangent(float twist, Vector3 tangent)
            {
                this.tangent = tangent;
                normal = new Vector3(-tangent.y, tangent.x, 0).normalized;
                binormal = Vector3.Cross(tangent, normal);

                Quaternion twistQ = Quaternion.AngleAxis(twist, tangent);
                normal = twistQ * normal;
                binormal = twistQ * binormal;
            }

            public void Transport(Vector3 newPosition, Vector3 newTangent, float twist)
            {

                // Calculate delta rotation:
                Quaternion rotQ = Quaternion.FromToRotation(tangent, newTangent);
                Quaternion twistQ = Quaternion.AngleAxis(twist, newTangent);
                Quaternion finalQ = twistQ * rotQ;

                // Rotate previous frame axes to obtain the new ones:
                normal = finalQ * normal;
                binormal = finalQ * binormal;
                tangent = newTangent;
                position = newPosition;

            }

            public void DrawDebug(float length)
            {
                Debug.DrawRay(position, normal * length, Color.blue);
                Debug.DrawRay(position, tangent * length, Color.red);
                Debug.DrawRay(position, binormal * length, Color.green);
            }
        }

        private MeshFilter filter;
        private Mesh mesh;

        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector4> tangents = new List<Vector4>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<int> tris = new List<int>();

        private CurveFrame frame;
        public RopeSections sections;

        private List<Vector3> _array;

        public Vector2 uvScale = Vector2.one;
        public float thickness = 0.025f;
        public int smooth = 1;

        public float uvSize;
        public float uvOffset = 1;
        public float strain = 0.5f;

        public void OnEnable()
        {
            filter = GetComponent<MeshFilter>();

            mesh = new Mesh();
            mesh.name = "cable";
            mesh.MarkDynamic();
            filter.mesh = mesh;
        }

        public void SetPoints(List<Vector3> p)
        {
            _array = p;
        }

        public void OnDisable()
        {
            DestroyImmediate(mesh);
        }

        public float GetLength(List<Vector3> array)
        {
            var l = 0f;

            for (var index = 0; index < array.Count; index++)
            {
                if (index < array.Count - 1)
                    l += Vector3.Distance(array[index], array[index + 1]);
            }

            return l;

        }

        public void LateUpdate()
        {
            ClearMeshData();

            if (_array.Count < 2) return;

            List<Vector3> finalPoints;

            if (smooth > 0)
            {
                finalPoints = LineSmoother.SmoothList(new List<Vector3>(_array), smooth);
            }
            else
            {
                finalPoints = new List<Vector3>(_array);
            }

            if (sections == null)
                return;

            int sectionSegments = sections.Segments;
            int verticesPerSection =
                sectionSegments + 1;

            float vCoord = -uvScale.y * GetLength(_array);
            int sectionIndex = 0;

            if (frame == null)
                frame = new CurveFrame();

            Vector4 texTangent;
            Vector2 uv = Vector2.zero;

            Matrix4x4 w2l = transform.worldToLocalMatrix;

            var length = GetLength(finalPoints);
            var m = length / (float)uvSize;
            var rest = m * uvOffset;

            var ux = 0f;
            var uy = 0f;
            for (int i = 0; i < finalPoints.Count; ++i)
            {
                ux += rest;
                uy += rest;

                frame.Reset();
                // Calculate previous and next curve indices:
                int nextIndex = Mathf.Min(i + 1, finalPoints.Count - 1);
                int prevIndex = Mathf.Max(i - 1, 0);

                Vector3 point = w2l.MultiplyPoint3x4(finalPoints[i]);
                Vector3 prevPoint = w2l.MultiplyPoint3x4(finalPoints[prevIndex]);
                Vector3 nextPoint = w2l.MultiplyPoint3x4(finalPoints[nextIndex]);

                Vector3 nextV = (nextPoint - point).normalized;
                Vector3 prevV = (point - prevPoint).normalized;
                Vector3 tangent = nextV + prevV;

                // update frame:
                frame.Transport(point, tangent, 0);

                // advance v texcoord:
                vCoord += uvScale.y * (Vector3.Distance(point, prevPoint) / strain);

                // Loop around each segment:
                for (int j = 0; j <= sectionSegments; ++j)
                {
                    vertices.Add(frame.position +
                                 (sections.vertices[j].x * frame.normal + sections.vertices[j].y * frame.binormal) *
                                 thickness);
                    normals.Add(vertices[vertices.Count - 1] - frame.position);
                    texTangent = -Vector3.Cross(normals[normals.Count - 1], frame.tangent);
                    texTangent.w = 1;
                    tangents.Add(texTangent);

                    //uv.Set(vCoord, (j / (float)sectionSegments) * uvScale.x);
                    uv.Set(ux, uy);
                    //  uv.Set(point.y, point.x);
                    uvs.Add(uv);

                    if (j < sectionSegments && i < finalPoints.Count - 1)
                    {

                        tris.Add(sectionIndex * verticesPerSection + j);
                        tris.Add(sectionIndex * verticesPerSection + (j + 1));
                        tris.Add((sectionIndex + 1) * verticesPerSection + j);

                        tris.Add(sectionIndex * verticesPerSection + (j + 1));
                        tris.Add((sectionIndex + 1) * verticesPerSection + (j + 1));
                        tris.Add((sectionIndex + 1) * verticesPerSection + j);

                    }
                }

                sectionIndex++;
            }


            CommitMeshData();
        }

        private void ClearMeshData()
        {
            mesh.Clear();
            vertices.Clear();
            normals.Clear();
            tangents.Clear();
            uvs.Clear();
            tris.Clear();
        }

        private void CommitMeshData()
        {
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0, true);

            OnUpdateEvent?.Invoke();
        }
    }
}