using UnityEngine;

public class UnitGravity : MonoBehaviour
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

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private UnitController _unitAnimations;
    private UnitArmature _currentUnit;
    private ART_Inputs _input;

    public float getVerticalVelocity { get { return _verticalVelocity; } }
    public UnitArmature setCurrentUnit { set { _currentUnit = value; } }

    private void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    public void Init(UnitController unitAnimator, ART_Inputs input)
    {
        _unitAnimations = unitAnimator;
        _input = input;
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
            PocessJumping();
        }
        else
        {
            ProcessFalling();
        }

        ApplyGravityOverTime();
    }

    private void PocessJumping()
    {
        ResetFallTimeoutTimer();
        _unitAnimations.ResetGravityBasedAnimations();
        StopVelocityDropping();
        Jump();
        ApplyJumpTimeout();
    }

    private void ResetFallTimeoutTimer()
    {
        // reset the fall timeout timer
        _fallTimeoutDelta = FallTimeout;
    }

    private void StopVelocityDropping()
    {
        // stop our velocity dropping infinitely when grounded
        if (_verticalVelocity < 0.0f)
        {
            _verticalVelocity = -2f;
        }
    }

    private void Jump()
    {
        // Jump
        if (_input.getJump && _jumpTimeoutDelta <= 0.0f)
        {
            CalculateJumpVelocity();
            _unitAnimations.ApplyJumpAnimation();
        }
    }

    private void CalculateJumpVelocity()
    {
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    }

    private void ApplyJumpTimeout()
    {
        // jump timeout
        if (_jumpTimeoutDelta >= 0.0f)
        {
            _jumpTimeoutDelta -= Time.deltaTime;
        }
    }

    private void ProcessFalling()
    {
        ResetJumpTimeoutTimer();
        ApplyFallTimeout();
        PreventJumping();
    }

    private void ResetJumpTimeoutTimer()
    {
        // reset the jump timeout timer
        _jumpTimeoutDelta = JumpTimeout;
    }

    private void ApplyFallTimeout()
    {
        // fall timeout
        if (_fallTimeoutDelta >= 0.0f)
        {
            _fallTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _unitAnimations.ApplyFreeFallAnimation();
        }
    }

    private void PreventJumping()
    {
        // if we are not grounded, do not jump
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
