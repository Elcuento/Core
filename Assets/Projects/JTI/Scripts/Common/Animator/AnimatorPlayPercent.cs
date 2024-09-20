using UnityEngine;

public class AnimatorPlayPercent : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _name;
    [SerializeField] private float _time;

    void Start()
    {
        _animator.Play(_name, 0, _time);
        Destroy(this);
    }
}
