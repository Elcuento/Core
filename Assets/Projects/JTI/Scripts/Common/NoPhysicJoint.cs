using JTI.Scripts.Common;
using UnityEngine;

namespace JTI.Scripts.Common
{
    public class NoPhysicJoint : MonoBehaviour
    {
        public Transform Connected;

        public Vector3 CurrentVelocity;
        public Vector3 MaxVelocity;

        public float MaxRotateVelocity;
        public float CurrentRotateVelocity;

        private Vector3 _rotationTarget;
        private Vector3 _currentRotation;

        private Vector3 _lastAngle;
        private Vector3 _lastPos;

        private Quaternion _lastRotate;

        public bool IsLimitRotation;

        public float LimitAngle;


        private void Start()
        {

            _lastPos = transform.position;
            _lastAngle = Vector3.zero;
        }


        void LateUpdate()
        {

            if (_lastAngle != Connected.transform.right)
            {
                var angle = Vector3.Angle(_lastAngle, Connected.transform.right);

                var s = angle / Time.deltaTime;

                MaxRotateVelocity = s;

                _lastAngle = Connected.transform.right;

            }
            else
            {
                MaxRotateVelocity = Mathf.Lerp(MaxRotateVelocity, 0, Time.deltaTime);
            }

            MaxRotateVelocity -= MaxRotateVelocity * Time.deltaTime;

            CurrentRotateVelocity = Mathf.Lerp(CurrentRotateVelocity, MaxRotateVelocity, Time.deltaTime);

            if (transform.position != _lastPos)
            {
                transform.rotation = _lastRotate;

                var dir = (_lastPos - transform.position).normalized;

                MaxVelocity = dir * Vector3.Distance(_lastPos, transform.position) / Time.deltaTime;

                _lastRotate = transform.rotation;
                _lastPos = transform.position;
            }
            else
            {
                MaxVelocity = Vector3.Lerp(MaxVelocity, new Vector3(0, MaxVelocity.y, 0), Time.deltaTime);
            }

            MaxVelocity -= Vector3.up * 9.8f * Time.deltaTime;

            CurrentVelocity = Vector3.Lerp(CurrentVelocity, MaxVelocity, Time.deltaTime);

            _rotationTarget = transform.position + CurrentVelocity;
            _currentRotation = Vector3.Lerp(_currentRotation, _rotationTarget, Time.deltaTime * 15);

            Utils.RotateZExtraOffset(transform, _currentRotation, 90 + CurrentRotateVelocity);


            if (IsLimitRotation)
            {
                transform.localEulerAngles = new Vector3(0, 0, ClampAngle(transform.transform.localEulerAngles.z, -LimitAngle, LimitAngle));
            }

        }

        protected float ClampAngle(float angle, float min, float max)
        {
            angle = NormalizeAngle(angle);
            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }

            min = NormalizeAngle(min);
            if (min > 180)
            {
                min -= 360;
            }
            else if (min < -180)
            {
                min += 360;
            }

            max = NormalizeAngle(max);
            if (max > 180)
            {
                max -= 360;
            }
            else if (max < -180)
            {
                max += 360;
            }

            // Aim is, convert angles to -180 until 180.
            return Mathf.Clamp(angle, min, max);
        }

        /** If angles over 360 or under 360 degree, then normalize them.
         */
        protected float NormalizeAngle(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }
    }

}
