using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(ART_Inputs), typeof(PlayerInput), typeof(UnitSwitcher))]
[RequireComponent(typeof(UnitGravity), typeof(UnitMovement), typeof(UnitCamera))]
public class UnitController : MonoBehaviour
{
    [SerializeField] private UnitArmature _currentUnit;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private bool _hasAnimator;

    private Animator _animator;
    private ART_Inputs _input;

    private UnitSwitcher _unitSwitcher;
    private UnitGravity _unitGravity;
    private UnitMovement _unitMovement;
    private UnitCamera _unitCamera;

    private void Awake()
    {
        _input = GetComponent<ART_Inputs>();
        _unitSwitcher = GetComponent<UnitSwitcher>();
        _unitGravity = GetComponent<UnitGravity>();
        _unitMovement = GetComponent<UnitMovement>();
        _unitCamera = GetComponent<UnitCamera>();
    }

    private void Start()
    {
        _unitSwitcher.Init(_input, InitUnitArmature);
        _unitGravity.Init(this, _input);
        _unitMovement.Init(this, _input, _unitGravity);
        _unitCamera.Init(_input);
        InitUnitArmature(_unitSwitcher.getNewUnitArmature);

        AssignAnimationIDs();
    }

    private void InitUnitArmature(UnitArmature currentUnit)
    {
        if (_currentUnit != null)
        {
            _currentUnit.EnableUnitCamera(false);
        }

        _currentUnit = currentUnit;
        _unitGravity.setCurrentUnit = currentUnit;
        _unitMovement.setCurrentUnit = currentUnit;
        _currentUnit.EnableUnitCamera(true);

        _unitCamera.UpdateCameraTarget(_currentUnit.getCameraRoot);
        
        _animator = _currentUnit.getAnimator;
        _hasAnimator = _animator != null;
        _unitMovement.setController = _currentUnit.getController;
    }

    private void Update()
    {
        _hasAnimator = _animator != null;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    // Animation Public Methods
    // TODO: Encapsulate into a separate class
    public void ApplyMovementAnimation(float animationBlend, float inputMagnitude)
    {
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    public void ResetGravityBasedAnimations()
    {
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);
        }
    }

    public void ApplyJumpAnimation()
    {
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDJump, true);
        }
    }

    public void ApplyFreeFallAnimation()
    {
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDFreeFall, true);
        }
    }

    public void ApplyGroundedAnimation(bool Grounded)
    {
        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }
}
