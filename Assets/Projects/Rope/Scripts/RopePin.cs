using UnityEngine;

public class RopePin : MonoBehaviour
{

    private GameObject _obj;
    
    private void OnCollisionExit(Collision col)
    {

        if (_obj == col.gameObject)
            _obj = col.gameObject;
    }

    private void OnCollisionStay(Collision col)
    {
        if (_obj == null)
            _obj = col.gameObject;

    }

    private void Update()
    {
        if(_obj != null)
        JTI.Scripts.Common.Utils.RotateZ(transform, _obj.transform.position);
    }
}
