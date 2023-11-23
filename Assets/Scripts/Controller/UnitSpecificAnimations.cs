using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator))]
public class UnitSpecificAnimations : MonoBehaviour
{
    [SerializeField] private BasicRigidBodyPush _bodyPush;

    private int _animIDLightAttack;

    private int _currentAnimID;

    private bool _inAnimation;
    private bool _isOpportunityWindow;

    private Animator _animator;
    private LockOnSystem _lockOnSystem;

    public bool getInAnimation { get { return _inAnimation; } }
    public bool getIsOpportunityWindow { get {  return _isOpportunityWindow; } }

    [Inject]
    public void InjectDependencies(LockOnSystem lockOnSystem)
    {
        _lockOnSystem = lockOnSystem;
    }

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
            AdjustPositionToLockedTarget();
            _animator.SetBool(_animIDLightAttack, true);
        }
    }

    private void AdjustPositionToLockedTarget()
    {
        if (_lockOnSystem != null)
        {
            _lockOnSystem.LockOnCurrentTarget();
        }
    }

    // Used by Animation Events
    private void OnAnimationStarted()
    {
        _inAnimation = true;
        _animator.applyRootMotion = _inAnimation;
        _animator.SetBool(_currentAnimID, !_inAnimation);
        _bodyPush.setCanPush = _inAnimation;
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
        _bodyPush.setCanPush = _inAnimation;
    }
}
