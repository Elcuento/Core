using System.Collections.Generic;
using Assets.Rope;
using UnityEngine;

public class RopeSegment : MonoBehaviour
{
    [SerializeField] private bool _fix;
    [SerializeField] private List<Rigidbody2D> _attach;

    public GameObject Next;
    public GameObject Prev;

    public Rigidbody2D Rigid;
    public HingeJoint2D Joint;

    public Rope Rope;

    private void Awake()
    {
       // Joint = GetComponent<HingeJoint2D>();

        Rigid.centerOfMass = Vector3.zero;

        if (_fix)
        {
            Rigid.constraints = RigidbodyConstraints2D.FreezeAll;
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

            var j = d.GetComponents<HingeJoint2D>();

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

    public void Init(Rope rope, GameObject prev, GameObject next)
    {
        Next = next;
        Prev = prev;

        Rope = rope;

        Rigid.constraints = RigidbodyConstraints2D.None;
        //      Joint = GetComponent<HingeJoint2D>();
    }

    public void Push(Vector3 pushPower)
    {
        Rigid.AddForce(pushPower);
    }
}
