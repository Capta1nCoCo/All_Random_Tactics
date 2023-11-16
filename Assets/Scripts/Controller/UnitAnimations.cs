using UnityEngine;

public class UnitAnimations : MonoBehaviour
{
    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private bool _hasAnimator;

    private Animator _animator;

    private void Start()
    {
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public void SetNewAnimator(Animator animator)
    {
        _animator = animator;
        _hasAnimator = _animator != null;
    }

    private void Update()
    {
        _hasAnimator = _animator != null;
    }

    public void ApplyMovementAnimation(float animationBlend, float inputMagnitude)
    {
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    public void ResetGravityBasedAnimations()
    {
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);
        }
    }

    public void ApplyJumpAnimation()
    {
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDJump, true);
        }
    }

    public void ApplyFreeFallAnimation()
    {
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDFreeFall, true);
        }
    }

    public void ApplyGroundedAnimation(bool Grounded)
    {
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }
}