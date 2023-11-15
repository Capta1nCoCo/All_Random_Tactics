using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float SpeedChangeRate = 10.0f;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;

    private ART_Inputs _input;
    private UnitAnimations _unitAnimations;

    private GameObject _mainCamera;
    private UnitGravity _unitGravity;

    private CharacterController _controller;
    private UnitArmature _currentUnit;

    public CharacterController setController { set { _controller = value; } }
    public UnitArmature setCurrentUnit { set { _currentUnit = value; } }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    public void Init(UnitAnimations animations, ART_Inputs input, UnitGravity unitGravity)
    {
        _unitAnimations = animations;
        _input = input;
        _unitGravity = unitGravity;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.getSprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.getMove == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.getAnalogMovement ? _input.getMove.magnitude : 1f;

        ProcessMovement(targetSpeed, currentHorizontalSpeed, speedOffset, inputMagnitude);
    }

    private void ProcessMovement(float targetSpeed, float currentHorizontalSpeed, float speedOffset, float inputMagnitude)
    {
        ApplyAcceleration(targetSpeed, currentHorizontalSpeed, speedOffset, inputMagnitude);
        CalculateAnimationBlend(targetSpeed);
        RotateTowardsInputDirection();
        MoveInTargetDirection();
        _unitAnimations.ApplyMovementAnimation(_animationBlend, inputMagnitude);
    }

    private void ApplyAcceleration(float targetSpeed, float currentHorizontalSpeed, float speedOffset, float inputMagnitude)
    {
        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
    }

    private void CalculateAnimationBlend(float targetSpeed)
    {
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
    }

    private void RotateTowardsInputDirection()
    {
        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.getMove.x, 0.0f, _input.getMove.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.getMove != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(_currentUnit.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            _currentUnit.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    private void MoveInTargetDirection()
    {
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _unitGravity.getVerticalVelocity, 0.0f) * Time.deltaTime);
    }
}
