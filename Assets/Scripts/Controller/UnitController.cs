using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(ART_Inputs), typeof(PlayerInput), typeof(UnitSwitcher))]
[RequireComponent(typeof(UnitGravity), typeof(UnitMovement))]
public class UnitController : MonoBehaviour
{
    [SerializeField] private UnitArmature _currentUnit;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    private const float _threshold = 0.01f;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private bool _hasAnimator;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private Animator _animator;
    private ART_Inputs _input;

    private UnitSwitcher _unitSwitcher;
    private UnitGravity _unitGravity;
    private UnitMovement _unitMovement;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    private void Awake()
    {
        _input = GetComponent<ART_Inputs>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
        _unitSwitcher = GetComponent<UnitSwitcher>();
        _unitGravity = GetComponent<UnitGravity>();
        _unitMovement = GetComponent<UnitMovement>();
    }

    private void Start()
    {
        _unitSwitcher.Init(_input, InitUnitArmature);
        _unitGravity.Init(this, _input);
        _unitMovement.Init(this, _input, _unitGravity);
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

        CinemachineCameraTarget = _currentUnit.getCameraRoot;
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _animator = _currentUnit.getAnimator;
        _hasAnimator = _animator != null;
        _unitMovement.setController = _currentUnit.getController;
    }

    private void Update()
    {
        _hasAnimator = _animator != null;

        //Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_input.getLook.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.getLook.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.getLook.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
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
