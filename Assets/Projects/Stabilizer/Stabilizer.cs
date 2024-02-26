//by Anji Games 2018
using UnityEngine;

public class Stabilizer : MonoBehaviour
{

    public float mass = 1;
    public Transform _movePart;

    private Vector3 stabPOS = new Vector3();
    private Vector3 stabVEL = new Vector3();
    private Transform _root;

    void OnEnable()
    {
        stabPOS = transform.position;
        _root = transform;
    }

    void Update()
    {
        stabPOS = Vector3.Lerp(stabPOS, _root.position, Time.deltaTime * 3 * mass) + stabVEL * 10 * mass * Time.deltaTime;
        var locPos = transform.InverseTransformPoint(stabPOS);
        _movePart.localEulerAngles = new Vector3(locPos.z - _root.localPosition.z, 0, -(locPos.x - _root.localPosition.x)) * 6 * mass;
        stabVEL = Vector3.Lerp(stabVEL, _root.position - stabPOS, Time.deltaTime * 5);
    }
}