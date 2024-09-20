using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimatorPlaySpeed : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed = 1;
    
    void Start()
    {
        if (_animator != null)
        {
            _animator.speed = _speed;
        }
        else
        {
            Debug.LogError(SceneManager.GetActiveScene().name + "No animator on ");
        }

        Destroy(this);
    }
}
