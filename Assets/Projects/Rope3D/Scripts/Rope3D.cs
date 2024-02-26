using System;
using System.Collections.Generic;
using JTI.Scripts.Common;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
#endif

namespace JTI.Projects.Rope3D
{
    public class Rope3D : MonoBehaviour
    {
        [SerializeField] string _pathToRope;
        [SerializeField] Rope3DMeshRender _render;
        [SerializeField] RopeSegment3D _segmentPrefab;
        [SerializeField] Transform _container;
        [SerializeField] List<RopeSegment3D> _segments;
        [SerializeField] private float _smooth;
        [SerializeField] private float _breakForce;

        public List<RopeSegment3D> Segments => _segments;

        [SerializeField] private float _segmentSpacing;
        public float SegmentSpacing => _segmentSpacing;


        private void OnValidate()
        {
            Draw();
        }
        public void InitRope(bool breakRope = false)
        {
            for (var index = 0; index < Segments.Count; index++)
            {
                var part = Segments[index];

                part.name = index.ToString();

                var prev = index > 0 ? Segments[index - 1] : null;
                var next = index < Segments.Count - 1 ? Segments[index + 1] : null;

                // DestroyJoint(part);

                if (index != 0)
                {
                    //  CreateJoint(part, Segments[index - 1]);
                }

                if (index < Segments.Count - 1 && next != null)
                {
                    part.transform.rotation =
                        Quaternion.LookRotation((next.transform.position - part.transform.position).normalized,
                            Vector3.forward);
                }
                else
                {
                    if (prev != null)
                    {
                        part.transform.rotation =
                            Quaternion.LookRotation((part.transform.position - prev.transform.position).normalized,
                                Vector3.forward);
                    }

                }

                part.Init(this, prev?.gameObject, next?.gameObject, _segmentSpacing);
            }

            for (var index = 0; index < Segments.Count; index++)
            {
                var part = Segments[index];

                DestroyJoint(part);

                if (index != 0)
                {
                    CreateJoint(part, Segments[index - 1]);
                }
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


        public void Cut(RopeSegment3D s)
        {
            if (_breakForce == -1)
                return;

            var seg = Segments.IndexOf(s);

            Cut(seg);
        }

        public void Cut(int pos)
        {
            var from = Segments.Count - pos;

            var cutCount = Segments.Count - from - 1;

            if ((from <= 1 && cutCount <= 1) || _segments.Count <= 3) return;

            var list = new List<RopeSegment3D>();
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


        public void Copy(List<RopeSegment3D> segments)
        {
            var o = Instantiate(Resources.Load<Rope3D>(_pathToRope));
            o.name = Random.Range(0, 9999).ToString();
            o.Init(segments, _breakForce);
        }

        public void Init(List<RopeSegment3D> rope, float b)
        {
            _segments = rope;
            _breakForce = b;

            for (var index = 0; index < _segments.Count; index++)
            {
                _segments[index].transform.SetParent(_container);
            }

            InitRope();
        }

        public void SpawnNoInit(Vector3 place)
        {
            if (Segments == null) _segments = new List<RopeSegment3D>();

            RopeSegment3D tmp = null;

            tmp = Utils.SpawnPrefabOfInstance(_segmentPrefab);
            tmp.transform.position = place;
            tmp.transform.SetParent(_container);

            tmp.name = (_segments.Count + 1).ToString();

            _segments.Add(tmp);

            ReCalculateDistance();
        }

        public void Spawn(Vector3 place)
        {
            if (Segments == null) _segments = new List<RopeSegment3D>();

            RopeSegment3D tmp = null;

            tmp = Utils.SpawnPrefabOfInstance(_segmentPrefab);
            tmp.transform.position = place;
            tmp.transform.SetParent(_container);


            tmp.name = (_segments.Count + 1).ToString();

            _segments.Add(tmp);

            ReCalculateDistance();
            InitRope();
        }

        public void DestroyJoint(RopeSegment3D from)
        {
            if (from.Joint != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(from.Joint);
                else Destroy(from.Joint);
            }

            from.Joint = null;

        }

        public void CreateJoint(RopeSegment3D from, Rigidbody to, bool autoConfig = true)
        {
            if (from.Joint != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(from.Joint);
                else Destroy(from.Joint);
            }

            from.Joint = to.gameObject.AddComponent<ConfigurableJoint>();
            from.Joint.autoConfigureConnectedAnchor = true;
            from.Joint.anchor = to.transform.InverseTransformPoint(from.transform.position);
            from.Joint.connectedBody = from.Rigid;
            from.Joint.breakForce = _breakForce <= 0 ? float.PositiveInfinity : _breakForce;
            from.Joint.axis = new Vector3(0, 0, -1);
            from.Joint.anchor = new Vector3(0, 0, -_segmentSpacing * 2);
            from.Joint.enablePreprocessing = true;
            from.Joint.projectionMode = JointProjectionMode.PositionAndRotation;
            from.Joint.xMotion = ConfigurableJointMotion.Locked;
            from.Joint.yMotion = ConfigurableJointMotion.Locked;
            from.Joint.zMotion = ConfigurableJointMotion.Locked;
        }

        public void CreateJoint(RopeSegment3D from, RopeSegment3D to)
        {
            //  Debug.Log("Create jount " + name +":" + from +":" + to);
            if (from.Joint != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(from.Joint);
                else Destroy(from.Joint);
            }

            //  from.Joint = null;
            from.Joint = from.gameObject.AddComponent<ConfigurableJoint>();

            from.Joint.autoConfigureConnectedAnchor = false;
            from.Joint.connectedAnchor = new Vector2(0, 0);
            from.Joint.anchor = new Vector3(0, 0, -_segmentSpacing * 2);
            from.Joint.axis = new Vector3(0, 0, -1);
            from.Joint.connectedBody = to.Rigid;
            from.Joint.breakForce = _breakForce <= 0 ? float.PositiveInfinity : _breakForce;
            from.Joint.enablePreprocessing = true;
            from.Joint.projectionMode = JointProjectionMode.PositionAndRotation;
            from.Joint.xMotion = ConfigurableJointMotion.Locked;
            from.Joint.yMotion = ConfigurableJointMotion.Locked;
            from.Joint.zMotion = ConfigurableJointMotion.Locked;
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

        public void DestroySegment(RopeSegment3D s)
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
                var RopeSegment3D = Segments[index];
                if (RopeSegment3D == null)
                {
                    Segments.Remove(Segments[index]);
                    index--;
                }
            }
            //InitRope();
#endif
        }


        private void LateUpdate()
        {
            Draw();
        }

        public void AligX()
        {
            var x = Segments[0].transform.position.x;
            foreach (var RopeSegment3D in Segments)
            {
                RopeSegment3D.transform.position = new Vector3(x, RopeSegment3D.transform.position.y);
                RopeSegment3D.transform.eulerAngles = new Vector3(0, 0, -90);
            }

            ReCalculateDistance();
            Draw();
        }

        public void AligY()
        {
            var x = Segments[0].transform.position.y;
            foreach (var RopeSegment3D in Segments)
            {
                RopeSegment3D.transform.position = new Vector3(RopeSegment3D.transform.position.x, x);
            }

            ReCalculateDistance();
            Draw();
        }

        public void Draw()
        {
            var poses = _segments.ConvertAll(x => x.transform.position);

            _render.thickness = _segmentSpacing / 2f;
            _render.SetPoints(poses);
        }

        public void PushSegments(Vector3 power)
        {
            foreach (var RopeSegment3D in Segments)
            {
                RopeSegment3D.Push(power);
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
                SpawnNoInit(nextPos);
                nextPos += dir * _segmentSpacing;
                distanceLeft -= (dir * _segmentSpacing).magnitude;
            }

            SpawnNoInit(nextPos);

            if (_segmentSpacing / 2f < distanceLeft)
            {
                nextPos += dir * _segmentSpacing;
                SpawnNoInit(nextPos);
            }

            InitRope();
        }

    }
}
