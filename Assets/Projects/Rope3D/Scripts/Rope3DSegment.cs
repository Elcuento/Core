using System.Collections.Generic;
using UnityEngine;

namespace JTI.Projects.Rope3D
{
    public class Rope3DSegment : MonoBehaviour
    {
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private SphereCollider _colliderTrigger;

        [SerializeField] private bool _fix;
        [SerializeField] private List<Rigidbody> _attach;

        public GameObject Next;
        public GameObject Prev;

        public Rigidbody Rigid;
        public ConfigurableJoint Joint;

        public Rope3D Rope;

        private void Awake()
        {
            Rigid.centerOfMass = Vector3.zero;

            if (_fix)
            {
                Rigid.constraints = RigidbodyConstraints.FreezeAll;
            }

            for (var index = 0; index < _attach.Count; index++)
            {
                var d = _attach[index];
                if (d == null)
                {
                    _attach.Remove(_attach[index]);
                    index--;
                    continue;
                }

                var j = d.GetComponents<ConfigurableJoint>();

                for (var i = 0; i < j.Length; i++)
                {
                    if (j[i].connectedBody.gameObject == gameObject)
                        return;
                }

                Rope.CreateJoint(this, d, true);
            }
        }


        public void Reap()
        {
            Rope.Cut(this);
        }

        public void Init(Rope3D rope, GameObject prev, GameObject next, float spacing)
        {
            Next = next;
            Prev = prev;

            Rope = rope;

            Rigid.constraints = RigidbodyConstraints.None;

            _collider.radius = spacing;
            _colliderTrigger.radius = spacing;
        }

        public void Push(Vector3 pushPower)
        {
            Rigid.AddForce(pushPower);
        }
    }
}