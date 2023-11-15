using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCamera : MonoBehaviour
{
    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject _cinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField] private float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    [SerializeField] private bool LockCameraPosition = false;

    private const float _threshold = 0.01f;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    private ART_Inputs _input;

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
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    public void Init(ART_Inputs input)
    {
        _input = input;
    }

    public void SetNewCameraTarget(GameObject cinemachineCameraTarget)
    {
        _cinemachineCameraTarget = cinemachineCameraTarget;
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        TranslateInputToRotations();
        ClampRotations();
        FollowCameraTarget();
    }

    private void TranslateInputToRotations()
    {
        // if there is an input and camera position is not fixed
        if (_input.getLook.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.getLook.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.getLook.y * deltaTimeMultiplier;
        }
    }

    private void ClampRotations()
    {
        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
    }

    private void FollowCameraTarget()
    {
        // Cinemachine will follow this target
        _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
