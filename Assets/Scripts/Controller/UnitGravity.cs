using UnityEngine;
using Zenject;

public class UnitGravity : MonoBehaviour, ICurrentUnitUser
{
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool Grounded = true;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float FallTimeout = 0.15f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField] private float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float Gravity = -15.0f;

    [Tooltip("Useful for rough ground")]
    [SerializeField] private float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask GroundLayers;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private bool _lockJumping;

    private UnitAnimations _unitAnimations;
    private UnitArmature _currentUnit;
    private ART_Inputs _input;

    public bool getGrounded { get { return Grounded; } }
    public float getVerticalVelocity { get { return _verticalVelocity; } }

    [Inject]
    public void InjectDependencies(UnitAnimations animations, ART_Inputs input)
    {
        _unitAnimations = animations;
        _input = input;
    }

    private void Start()
    {
        ResetTimeouts();
    }

    private void ResetTimeouts()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    public void SetCurrentUnit(UnitArmature unit)
    {
        ResetPrevUnitGravityAnimations();
        _currentUnit = unit;
    }

    private void ResetPrevUnitGravityAnimations()
    {
        if (_currentUnit != null)
        {
            _unitAnimations.ResetGravityBasedAnimations();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnLockOn += OnLockOn;
    }

    private void OnDisable()
    {
        GameEvents.OnLockOn -= OnLockOn;
    }

    private void OnLockOn(bool locked)
    {
        _lockJumping = locked;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            PocessGravity();
        }
        else
        {
            ProcessFalling();
        }

        ApplyGravityOverTime();
    }

    private void PocessGravity()
    {
        ResetFallTimeoutTimer();
        _unitAnimations.ResetGravityBasedAnimations();
        StopVelocityDroppingWhenGrounded();
        Jump();
        ApplyJumpTimeout();
    }

    private void ResetFallTimeoutTimer()
    {
        _fallTimeoutDelta = FallTimeout;
    }

    private void StopVelocityDroppingWhenGrounded()
    {
        if (_verticalVelocity < 0.0f)
        {
            _verticalVelocity = -2f;
        }
    }

    private void Jump()
    {
        LockJumping();
        if (_input.getJump && _jumpTimeoutDelta <= 0.0f)
        {
            CalculateJumpVelocity();
            _unitAnimations.ApplyJumpAnimation();
        }
    }

    private void LockJumping()
    {
        if (_input.getJump && _lockJumping)
        {
            _input.setJump = false;
        }
    }

    private void CalculateJumpVelocity()
    {
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    }

    private void ApplyJumpTimeout()
    {
        if (_jumpTimeoutDelta >= 0.0f)
        {
            _jumpTimeoutDelta -= Time.deltaTime;
        }
    }

    private void ProcessFalling()
    {
        ResetJumpTimeoutTimer();
        ApplyFallTimeout();
        PreventJumpingWhileAirborne();
    }

    private void ResetJumpTimeoutTimer()
    {
        _jumpTimeoutDelta = JumpTimeout;
    }

    private void ApplyFallTimeout()
    {
        if (_fallTimeoutDelta >= 0.0f)
        {
            _fallTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _unitAnimations.ApplyFreeFallAnimation();
        }
    }

    private void PreventJumpingWhileAirborne()
    {
        _input.setJump = false;
    }

    private void ApplyGravityOverTime()
    {
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(_currentUnit.transform.position.x, _currentUnit.transform.position.y - GroundedOffset,
            _currentUnit.transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        _unitAnimations.ApplyGroundedAnimation(Grounded);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        if (_currentUnit == null) { return; }
        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(_currentUnit.transform.position.x, _currentUnit.transform.position.y - GroundedOffset, _currentUnit.transform.position.z),
            GroundedRadius);
    }
}