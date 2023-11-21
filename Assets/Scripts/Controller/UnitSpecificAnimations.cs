using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitSpecificAnimations : MonoBehaviour
{
    private int _animIDLightAttack;

    private int _currentAnimID;

    private bool _inAnimation;
    private bool _isOpportunityWindow;

    private Animator _animator;

    public bool getInAnimation { get { return _inAnimation; } }
    public bool getIsOpportunityWindow { get {  return _isOpportunityWindow; } }

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
            _isOpportunityWindow = false;
            _currentAnimID = _animIDLightAttack;
            _animator.SetBool(_animIDLightAttack, true);
        }
    }

    // Used by Animation Events
    private void OnAnimationStarted()
    {
        _inAnimation = true;
        _animator.applyRootMotion = _inAnimation;
        _animator.SetBool(_currentAnimID, !_inAnimation);
    }

    // Used by Animation Events
    private void OnOpportunityWindowOpened()
    {
        _isOpportunityWindow = true;
    }

    // Used by Animation Events
    private void OnOpportunityWindowClosed()
    {
        _isOpportunityWindow = false;
    }

    // Used by Animation Events
    private void OnAnimationFinished()
    {
        _inAnimation = false;
        _animator.applyRootMotion = _inAnimation;
    }
}
