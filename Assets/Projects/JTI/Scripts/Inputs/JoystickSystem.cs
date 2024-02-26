using JTI.Scripts;
using UnityEngine;

namespace JTI.Scripts
{
    public class JoystickSystem : MonoBehaviour
    {
        [SerializeField] private Joystick _joystick;

        public Joystick Joystick => _joystick;
    }

}