using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Animator))]
public class UnitSpecificAnimations : MonoBehaviour
{
    private const string LIGHT_ATTACK = "LightAttack";

    [SerializeField] private BasicRigidBodyPush _bodyPush;

    [Header("Motion Animation Parameters")]
    [SerializeField] float speed = 8f;
    [SerializeField] float stoppingDistance = 1.5f;

    private int _animIDLightAttack;

    private int _currentAnimID;

    private bool _inAnimation;
    private bool _isOpportunityWindow;

    private Animator _animator;
    private LockOnSystem _lockOnSystem;
    private CharacterController _characterController;

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
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _animIDLightAttack = Animator.StringToHash(LIGHT_ATTACK);
    }

    public void ApplyAbilityAnimation(string id)
    {
        if (_animator != null)
        {
            int abilityAnimID = Animator.StringToHash(id);
            _currentAnimID = abilityAnimID;
            _animator.SetBool(_currentAnimID, true);
        }
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
        _lockOnSystem.ReleaseCurrentTarget();
    }

    // Used by Animation Events
    private void OnMovementAnimation()
    {
        if (!_inAnimation)
        {
            _inAnimation = true;
            StartCoroutine(MoveTowardsTarget(_lockOnSystem.getCurrentTarget));
        }
    }

    private IEnumerator MoveTowardsTarget(Transform target)
    {
        const float gravity = -9.81f;
        Vector3 movementDirection = target.position - transform.position;
        float distance = Vector3.Distance(transform.position, target.position);
        while (distance > stoppingDistance)
        {
            distance = Vector3.Distance(transform.position, target.position);
            _characterController.Move(movementDirection.normalized * (speed * Time.deltaTime) + 
                new Vector3(0f, gravity, 0f) * Time.deltaTime);
            yield return null;
        }
        _animator.SetBool(_currentAnimID, !_inAnimation);
    }
}