using JTI.Scripts;
using UnityEngine;

public class JoystickSystem : MonoBehaviour
{
    [SerializeField] private Joystick _joystick;

    public Joystick Joystick => _joystick;
}
