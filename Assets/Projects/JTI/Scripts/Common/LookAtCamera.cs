using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JTI.Scripts.Common
{
    public class LookAtCamera : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }
    }

}
