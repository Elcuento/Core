using System;
using System.Collections.Generic;
using JTI.Scripts.Common;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
#endif

namespace JTI.Projects.Rope2D
{
    public class Rope : MonoBehaviour
    {
        [SerializeField] string _pathToRope;
        [SerializeField] RopeSegment _segmentPrefab;
        [SerializeField] private LineRenderer _render;
        [SerializeField] Transform _container;
        [SerializeField] List<RopeSegment> _segments;
        [SerializeField] private float _smooth;
        [SerializeField] private float _breakForce;

        public List<RopeSegment> Segments => _segments;

        [SerializeField] private float _segmentSpacing;
        public float SegmentSpacing => _segmentSpacing;


        public void InitRope(bool breakRope = false)
        {
            for (var index = 0; index < Segments.Count; index++)
            {
                var part = Segments[index];

                part.name = index.ToString();

                var prev = index > 0 ? Segments[index - 1] : null;
                var next = index < Segments.Count - 1 ? Segments[index + 1] : null;

                DestroyJoint(part);

                if (index != 0)
                {
                    CreateJoint(part, Segments[index - 1]);
                }

                if (index < Segments.Count - 1 && next != null)
                {
                    Utils.RotateZ(part.transform, next.transform.position);
                }
                else
                {
                    if (prev != null)
                        Utils.RotateReverseZ(part.transform, prev.transform.position);
                }

                part.Init(this, prev?.gameObject, next?.gameObject, _segmentSpacing);
            }

            Draw();

        }

        public void ReCalculateDistance()
        {
            for (var index = 0; index < Segments.Count; index++)
            {
                if (index != 0)
                {
                    if (Vector3.Distance(Segments[index].transform.position, Segments[index - 1].transform.position) > _segmentSpacing)
                    {
                        Segments[index].transform.position = Segments[index - 1]
                                                                 .transform.position - (Segments[index - 1].transform.position - Segments[index].transform.position).normalized * _segmentSpacing;
                    }
                }
            }
        }


        public void Cut(RopeSegment s)
        {
            if (_breakForce == -1)
                return;

            var seg = Segments.IndexOf(s);

            Cut(seg);
        }

        public static List<Vector3> SmoothLine(Vector3[] inputPoints, float segmentSize, float space, int max = 100)
        {
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            Keyframe[] keysX = new Keyframe[inputPoints.Length];
            Keyframe[] keysY = new Keyframe[inputPoints.Length];
            Keyframe[] keysZ = new Keyframe[inputPoints.Length];

            for (int i = 0; i < inputPoints.Length; i++)
            {
                keysX[i] = new Keyframe(i, inputPoints[i].x);
                keysY[i] = new Keyframe(i, inputPoints[i].y);
                keysZ[i] = new Keyframe(i, inputPoints[i].z);
            }

            curveX.keys = keysX;
            curveY.keys = keysY;
            curveZ.keys = keysZ;

            for (int i = 0; i < inputPoints.Length; i++)
            {
                if (curveX.keys.Length > i)
                    curveX.SmoothTangents(i, 0);

                if (curveY.keys.Length > i)
                    curveY.SmoothTangents(i, 0);

                if (curveZ.keys.Length > i)
                    curveZ.SmoothTangents(i, 0);
            }

            var lineSegments = new List<Vector3>();

            for (int i = 0; i < inputPoints.Length; i++)
            {
                lineSegments.Add(inputPoints[i]);

                if (i + 1 < inputPoints.Length)
                {
                    float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);

                    int segments = (int)(distanceToNext / segmentSize);

                    for (int s = 1; s < segments && s < max; s++)
                    {
                        float time = ((float)s / (float)segments) + (float)i;

                        Vector3 newSegment =
                            new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));

                        lineSegments.Add(newSegment);
                    }
                }
            }

            var size = inputPoints.Length;
            if (size > 1)
            {
                lineSegments.Insert(0, inputPoints[0] + (inputPoints[0] - inputPoints[1]).normalized * space / 2f);
                lineSegments.Add(inputPoints[size - 1] + (inputPoints[size - 1] - inputPoints[size - 2]).normalized * space / 2f);
            }

            return lineSegments;
        }

        public void Cut(int pos)
        {
            var from = Segments.Count - pos;

            var cutCount = Segments.Count - from - 1;

            if ((from <= 1 && cutCount <= 1) || _segments.Count <= 3) return;

            var list = new List<RopeSegment>();
            for (var i = 0; i < cutCount + 1; i++)
            {
                if (list.Count == 0)
                {
                    DestroyJoint(_segments[i]);
                }

                list.Add(_segments[i]);
            }

            for (var index = 0; index < list.Count; index++)
            {
                _segments.Remove(list[index]);
            }

            Copy(list);

            if (Segments.Count > 0)
            {
                Segments[0].Prev = null;
                DestroyJoint(Segments[0]);
            }

        }


        public void Copy(List<RopeSegment> segments)
        {
            var o = Instantiate(Resources.Load<Rope>(_pathToRope));
            o.name = Random.Range(0, 9999).ToString();
            o.Init(segments, _breakForce);
        }

        public void Init(List<RopeSegment> rope, float b)
        {
            _segments = rope;
            _breakForce = b;

            for (var index = 0; index < _segments.Count; index++)
            {
                _segments[index].transform.SetParent(_container);
            }

            InitRope();
        }

        public void Spawn(Vector3 place)
        {

            if (Segments == null) _segments = new List<RopeSegment>();

            RopeSegment tmp = null;

            tmp = Utils.SpawnPrefabOfInstance(_segmentPrefab);
            tmp.transform.position = place;
            tmp.transform.SetParent(_container);


            tmp.name = (_segments.Count + 1).ToString();

            _segments.Add(tmp);

            ReCalculateDistance();
            InitRope();
        }

        public void DestroyJoint(RopeSegment from)
        {
            if (from.Joint != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(from.Joint);
                else Destroy(from.Joint);
            }

            from.Joint = null;

        }

        public void CreateJoint(RopeSegment from, Rigidbody2D to, bool autoConfig = true)
        {
            var joint = to.gameObject.AddComponent<HingeJoint2D>();

            joint.autoConfigureConnectedAnchor = true;
            joint.anchor = to.transform.InverseTransformPoint(from.transform.position);
            joint.connectedBody = from.Rigid;
            joint.breakForce = _breakForce <= 0 ? float.PositiveInfinity : _breakForce;
        }

        public void CreateJoint(RopeSegment from, RopeSegment to)
        {
            //  Debug.Log("Create jount " + name +":" + from +":" + to);
            if (from.Joint != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(from.Joint);
                else Destroy(from.Joint);
            }

            from.Joint = null;
            from.Joint = from.gameObject.AddComponent<HingeJoint2D>();

            from.Joint.autoConfigureConnectedAnchor = false;
            from.Joint.connectedAnchor = new Vector2(0, 0);
            from.Joint.anchor = new Vector2(-_segmentSpacing * 2, 0);
            from.Joint.connectedBody = to.Rigid;
            from.Joint.breakForce = _breakForce <= 0 ? float.PositiveInfinity : _breakForce;
        }

        public float GetLength()
        {
            var l = 0f;

            for (var index = 0; index < Segments.Count; index++)
            {
                if (index > 0)
                {
                    l += Vector3.Distance(Segments[index].transform.position, Segments[index - 1].transform.position);
                }
            }

            return l;
        }

        public void Clear()
        {

            var listToDestroy = new List<GameObject>();
            for (int i = 0; i < _container.transform.childCount; i++)
            {
                listToDestroy.Add(_container.transform.GetChild(i).gameObject);
            }

            for (var index = 0; index < listToDestroy.Count; index++)
            {
                var o = listToDestroy[index];
                DestroyImmediate(o);
            }

            Segments.Clear();
            Draw();
        }

        public void DestroySegment(RopeSegment s)
        {
            if (Application.isPlaying)
                Destroy(s.gameObject);
            else DestroyImmediate(s.gameObject);

            Segments.Remove(s);

            InitRope(true);
        }

        public void DestroySegmentAt(int point)
        {
            if (Segments.Count > point && point >= 0 && Segments.Count != 0)
            {
                DestroySegment(Segments[point]);
            }
        }

        public void EditorUpdate()
        {
#if UNITY_EDITOR
            for (var index = 0; index < Segments.Count; index++)
            {
                var ropeSegment = Segments[index];
                if (ropeSegment == null)
                {
                    Segments.Remove(Segments[index]);
                    index--;
                }
            }
#endif
        }


        private void LateUpdate()
        {
            Draw();
        }

        public void AligX()
        {
            var x = Segments[0].transform.position.x;
            foreach (var ropeSegment in Segments)
            {
                ropeSegment.transform.position = new Vector3(x, ropeSegment.transform.position.y);
                ropeSegment.transform.eulerAngles = new Vector3(0, 0, -90);
            }

            ReCalculateDistance();
            Draw();
        }

        public void AligY()
        {
            var x = Segments[0].transform.position.y;
            foreach (var ropeSegment in Segments)
            {
                ropeSegment.transform.position = new Vector3(ropeSegment.transform.position.x, x);
            }

            ReCalculateDistance();
            Draw();
        }

        public void Draw()
        {
            var poses = _segments.ConvertAll(x => x.transform.position).ToArray();
            var p = SmoothLine(poses, _smooth, SegmentSpacing).ToArray();

            _render.positionCount = p.Length;
            _render.SetPositions(p);
        }

        public void PushSegments(Vector3 power)
        {
            foreach (var ropeSegment in Segments)
            {
                ropeSegment.Push(power);
            }
        }

        public void CreateRopeBetweenPoints(Vector3 transformPosition, Vector3 position)
        {
            Clear();

            var distanceLeft = Vector3.Distance(transformPosition, position);

            var dir = (position - transformPosition).normalized;
            var nextPos = transformPosition;

            while (distanceLeft > _segmentSpacing)
            {
                Spawn(nextPos);
                nextPos += dir * _segmentSpacing;
                distanceLeft -= (dir * _segmentSpacing).magnitude;
            }
        }
    }
}
