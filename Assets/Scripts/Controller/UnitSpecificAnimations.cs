using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitSpecificAnimations : MonoBehaviour
{
    private int _animIDLightAttack;

    private int _currentAnimID;

    private bool _inAnimation;

    private Animator _animator;

    public bool getInAnimation { get { return _inAnimation; } }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animIDLightAttack = Animator.StringToHash("LightAttack");
    }

    public void ApplyLightAttackAnimation()
    {
        if (_animator != null)
        {
            _currentAnimID = _animIDLightAttack;
            _animator.SetBool(_animIDLightAttack, true);
        }
    }

    // Used by Animation Events
    private void OnAnimationStarted()
    {
        _inAnimation = true;
        _animator.applyRootMotion = _inAnimation;
    }

    // Used by Animation Events
    private void OnAnimationFinished()
    {
        _inAnimation = false;
        _animator.SetBool(_currentAnimID, _inAnimation);
        _animator.applyRootMotion = _inAnimation;
    }
}
